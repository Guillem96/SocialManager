using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SocialManager_Server.Packets;
using SocialManager_Server.ClientLogic;
using System.Data.SqlClient;

namespace SocialManager_Server.ServerLogic
{
    static class TcpUtilities
    {
        public static void ReadyChatClient(byte[] data, TcpClient client, Server server)
        {
            string message = "";
            // Unpack the data
            BasicReqPacket packet = Packet.Unpack<BasicReqPacket>(data);

            // Get the client who is requesting
            ClientStatus c = server.GetClient(packet.Username);

            // Check the basics
            if (ClientsManagement.CheckBasics(c, ClientStatus.Status.Disconnected, packet.Alea, out message))
            {
                // Add client to dicctionary with his respective tcpclient
                server.ClientsOnChat.Add(c.Client.Username, client);

                server.DebugInfo("Client Ready for chat: " + c.Client.Username + " is ready for chatting.");

                // Send ack
                server.Tcp.SendMessage(new AckErrorPacket(PacketTypes.ReadyChatAck, "Now you are ready for chat").Pack(), client);
            }
            else
            {
                server.DebugInfo("Client Ready for chat error: " + message);
                server.Tcp.SendError(message, client);
            }
        }

        public static void SendMessage(byte[] data, TcpClient client, Server server)
        {
            try
            {
                string message = "";

                // Unpack the data
                MessagePacket packet = Packet.Unpack<MessagePacket>(data);

                server.DebugInfo("SendMessageReq Recieved: " + packet.ToString());

                // Who is making the query
                ClientStatus c = server.GetClient(packet.From.Username);

                // Check te basics again and again
                if (ClientsManagement.CheckBasics(c, ClientStatus.Status.Disconnected, packet.Alea, out message))
                {
                    // Check if destination exists
                    if (!server.Clients.Any(d => d.Client.Username == packet.To.Username))
                    {
                        server.DebugInfo("Send Message Error: " + "Destination doesn't exist");
                        server.Tcp.SendError("Destination doesn't exist", client);
                        return;
                    }

                    // Store the message on database
                    using (var db = new Models.ServerDatabase())
                    {
                            // Create the message 
                        Models.Message m = new Models.Message()
                        {
                            From = db.Clients.Single(n => n.Username == c.Client.Username),
                            To = db.Clients.Single(n => n.Username == packet.To.Username),
                            Date = packet.Date,
                            Content = packet.Content
                        };

                        // Check if destination is on chat
                        if (server.ClientsOnChat.Any(d => d.Key.Equals(packet.To.Username)))
                        {

                            // If exist save the message and send him as well as readed message
                            m.Read = true;
                            server.DebugInfo("Message from : " + packet.From.Username + " sended to: " + packet.To.Username);
                            // Send the message to destination changing the type of package
                            packet.Type = (byte)PacketTypes.SendMessageAck;
                            server.Tcp.SendMessage(packet.Pack(), server.ClientsOnChat[packet.To.Username]);
                        }
                        else
                        {
                            m.Read = false;
                        }

                    
                        db.Messages.InsertOnSubmit(m);
                        db.SubmitChanges();
                    }
                }
                else
                {
                    server.DebugInfo("Send Message Error: " + message);
                    server.Tcp.SendError(message, client);
                }
            }
            catch (SqlException)
            {
                server.DebugInfo("Send Message Error: database error.");
                server.Tcp.SendError("Database error", client);
            }
            
        }
    }
}
