using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SocialManager_Server.Packets;
using SocialManager_Server.ClientLogic;

namespace SocialManager_Server.ServerLogic
{
    static partial class UdpUtilities
    {
        public static void Alive(byte[] data, IPEndPoint ip, Server server)
        {
            string message = "";

            BasicReqPacket aPacket = Packet.Unpack<BasicReqPacket>(data);

            server.DebugInfo("Alive inf recieved.");
            server.DebugInfo("AliveInf Packet: " + aPacket.ToString());

            ClientStatus current = server.GetClient(aPacket.Username);

            if (ClientsManagement.CheckBasics(current, ClientStatus.Status.Disconnected, aPacket.Alea, out message))
            {
                // Save the last alive
                current.LastAlive = DateTime.Now;
                // Send ack
                server.Udp.SendMessage(new AckErrorPacket(PacketTypes.AliveAck, "Alive correct").Pack(), ip);
            }
            else
            {
                server.DebugInfo("Alive: Incorrect alive from " + aPacket.Username);
                server.DebugInfo(aPacket.Username + " now is disconnected.");

                if(server.ClientsOnChat.Any(d => d.Key == current.Client.Username))
                    server.ClientsOnChat.Remove(current.Client.Username);
                // Disconnect the client
                current.Disconnect();
                // Send error
                server.Udp.SendError(message, ip);
            }

        }

        public static void CheckAlives(Server server)
        {
            foreach (ClientStatus cs in server.Clients.Where(c => c.Stat != ClientStatus.Status.Disconnected))
            {
                TimeSpan since = DateTime.Now - cs.LastAlive;
                if (since.Seconds > 12)
                {
                    if (server.ClientsOnChat.Any(d => d.Key == cs.Client.Username))
                        server.ClientsOnChat.Remove(cs.Client.Username);
                    cs.Disconnect();
                    server.DebugInfo(cs.Client.Username + " now is disconnected because he is inactive.");
                }
            }
        }
    }
}
