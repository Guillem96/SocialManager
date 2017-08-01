using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.ServerLogic
{
    static partial class UdpUtilities
    {
        public static void LinkSocialNet(byte[] data, IPEndPoint ip, Server server) 
        {
            try {

                Packets.LinkSocialNetworkPacket packet = Packets.Packet.Unpack<Packets.LinkSocialNetworkPacket>(data);

                server.DebugInfo("Social Network Link: Request recieved");
                server.DebugInfo("Packet LinkSocialNetworkReq: " + packet.ToString());

                // Who is making the query
                ClientLogic.ClientStatus client = server.GetClient(packet.ClientUsername);

                // Check the basics of connection
                if (ClientLogic.ClientsManagement.CheckBasics(client, ClientLogic.ClientStatus.Status.Disconnected, packet.Alea, out string message))
                {
                    // Check if the link already exists
                    using (var db = new Models.ServerDatabase())
                    {
                        if (db.LinkedSocialNetworks
                                .Any(l => l.Client.Username.Equals(packet.ClientUsername) &&
                                        l.SocialNetwork.Name.Equals(packet.SocialNetName)))
                        {
                            // Link exists on database -> Override it
                            var sn = db.LinkedSocialNetworks.Single(l => l.Client.Username.Equals(packet.ClientUsername) &&
                                                                l.SocialNetwork.Name.Equals(packet.SocialNetName));

                            sn.Username = packet.SocialNetUsername;
                            sn.Password = packet.SocialNetPassword;

                            server.DebugInfo("Social Network Link: The link already exists, overriding it.");
                            return;
                        }
                        else
                        {
                            server.DebugInfo("Social Network Link: Social net linked correctly.");
                            db.LinkedSocialNetworks.InsertOnSubmit(new Models.LinkedSocialNetwork()
                            {
                                Client = db.Clients.Single(c => c.Username.Equals(packet.ClientUsername)),
                                SocialNetwork = db.SocialNetworks.Single(c => c.Name.Equals(packet.SocialNetName)),
                                Username = packet.SocialNetUsername,
                                Password = packet.SocialNetPassword
                            });
                        }

                        // Submit changes
                        db.SubmitChanges();

                        // Send the ack 
                        server.Udp.SendMessage(new Packets.AckErrorPacket(Packets.PacketTypes.LinkSocialNetworkAck,
                                                 "Social net linked correctly.").Pack(), ip);
                    }
                }
                else
                {
                    server.DebugInfo("Social Network Link: " + message);
                    server.Udp.SendError(message, ip);

                }
            }
            catch (SqlException e)
            {
                server.DebugInfo("Social Network Link: Database error.");
                server.Udp.SendError("Database error. Probably no social net found on database.", ip);
                return;
            }
        }

        public static void DeleteLinkSocialNet(byte[] data, IPEndPoint ip, Server server)
        {
            try
            {

                Packets.LinkSocialNetworkPacket packet = Packets.Packet.Unpack<Packets.LinkSocialNetworkPacket>(data);

                server.DebugInfo("Social Network Link Delete Request: Request recieved");
                server.DebugInfo("Packet DeleteLinkSocialNetworkReq: " + packet.ToString());

                // Who is making the query
                ClientLogic.ClientStatus client = server.GetClient(packet.ClientUsername);

                // Check the basics of connection
                if (ClientLogic.ClientsManagement.CheckBasics(client, ClientLogic.ClientStatus.Status.Disconnected, packet.Alea, out string message))
                {
                    // Check if the link already exists
                    using (var db = new Models.ServerDatabase())
                    {
                        if (db.LinkedSocialNetworks
                                .Any(l => l.Client.Username.Equals(packet.ClientUsername) &&
                                        l.SocialNetwork.Name.Equals(packet.SocialNetName)))
                        {
                            // If exist delete
                            db.LinkedSocialNetworks.DeleteOnSubmit(
                                                db.LinkedSocialNetworks
                                                                .Single(l => l.Client.Username.Equals(packet.ClientUsername) &&
                                                                            l.SocialNetwork.Name.Equals(packet.SocialNetName)));

                            server.DebugInfo("Delete Social Network Link: The link has been deleted.");

                            // Submit changes
                            db.SubmitChanges();

                            // Send the ack 
                            server.Udp.SendMessage(new Packets.AckErrorPacket(Packets.PacketTypes.DeleteLinkSocialNetAck,
                                                     "Social net deleted correctly.").Pack(), ip);
                            return;
                        }
                        else
                        {
                            server.DebugInfo("Delete Social Network Link: Social net doesn't exist, can't be deleted.");
                            // Send the ack 
                            server.Udp.SendMessage(new Packets.AckErrorPacket(Packets.PacketTypes.Error,
                                                     "Social net doesn't exist.").Pack(), ip);
                        }

                    }
                }
                else
                {
                    server.DebugInfo("Delete Social Network Link: " + message);
                    server.Udp.SendError(message, ip);

                }
            }
            catch (SqlException)
            {
                server.DebugInfo("Delete Social Network Link: Database error.");
                server.Udp.SendError("Database error. Probably no social net found on database.", ip);
                return;
            }

        }
    }
}
