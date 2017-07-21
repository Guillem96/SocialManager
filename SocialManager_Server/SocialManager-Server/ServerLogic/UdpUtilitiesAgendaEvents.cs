using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.ServerLogic
{
    static class UdpUtilitiesAgendaEvents
    {
        public static void AgendaEvent(byte[] data, Server server, bool delete, IPEndPoint ip)
        {
            Console.WriteLine(Encoding.ASCII.GetString(data));
            try
            {
                string message = "";
                // Unpack the data
                Packets.AgendaEventPacket packet = Packets.Packet.Unpack<Packets.AgendaEventPacket>(data);
                server.DebugInfo("Agenda Event Req: " + packet.ToString());

                // Get the client who is making the request
                ClientLogic.ClientStatus current = server.GetClient(packet.Username);

                // Check the basics
                if (ClientLogic.ClientsManagement.CheckBasics(current, ClientLogic.ClientStatus.Status.Disconnected, packet.Alea, out message))
                {
                    if (!delete)
                        AddEvent(packet, server, ip);
                    else
                        DeleteEvent(packet, server, ip);
               
                    
                }
                else
                {
                    server.DebugInfo("Adding agenda event error: " + message);
                    server.Udp.SendError(message, ip);
                }
            }
            catch (SqlException)
            {
                server.DebugInfo("Add new agenda event database error");
                server.Udp.SendError("Unexpected database error", ip);
            }
        }

        private static void AddEvent(Packets.AgendaEventPacket packet, Server server, IPEndPoint ip)
        {
            // Adding the event to database
            using (var db = new Models.ServerDatabase())
            {
                db.AgendaEvents.InsertOnSubmit(new Models.AgendaEvent()
                {
                    Client = db.Clients.Single(c => c.Username == packet.Username),
                    Date = packet.Date,
                    EventInfo = packet.EventInfo,
                    EventName = packet.EventName
                });
                db.SubmitChanges();
            }
            server.DebugInfo("Event added correctly by " + packet.Username);
            // Send the ack
            server.Udp.SendMessage(new Packets.AckErrorPacket(Packets.PacketTypes.NewAgendaEventAck, "New event added").Pack(), ip);
        }

        private static void DeleteEvent(Packets.AgendaEventPacket packet, Server server, IPEndPoint ip)
        {
            // Deleting the event of database
            using (var db = new Models.ServerDatabase())
            {
                if (db.AgendaEvents.Any(c => c.Client.Username == packet.Username &&
                                                        c.DateString.Equals(packet.DateString) &&
                                                        c.EventInfo.Equals(packet.EventInfo) &&
                                                        c.EventName.Equals(packet.EventName)))
                    db.AgendaEvents.DeleteOnSubmit(
                            db.AgendaEvents.Single(c => c.Client.Username == packet.Username &&
                                                            c.DateString.Equals(packet.DateString) &&
                                                            c.EventInfo.Equals(packet.EventInfo) &&
                                                            c.EventName.Equals(packet.EventName))

                        );
                else
                {
                    server.DebugInfo("Delete Agenda Event: Unable to find on the database.");
                    server.Udp.SendError("Unable to find on the database.", ip);
                    return;
                }

                server.DebugInfo("Delete Agenda Event: Deletd event correctly");
                server.Udp.SendMessage(new Packets.AckErrorPacket(Packets.PacketTypes.DeleteAgendaEventAck, "Event deleted").Pack(), ip);
            }
        }
    }
}
