using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client.UdpUtilities
{
    static class Agenda
    {
        public static bool AgendaEvent(string eventName, string eventInfo, DateTime date, bool delete, Client client, out string message)
        {
            try
            {
                // Pack register request packet
                Packets.AgendaEventPacket packet =
                            new Packets.AgendaEventPacket(
                                delete ? Packets.PacketTypes.DeleteAgendaEventReq : Packets.PacketTypes.NewAgendaEventReq,
                                client.Alea, //> For registering always 7 0's
                                client.Profile.Username,
                                eventName,
                                eventInfo,
                                date
                                );

                // Send register request package
                client.Udp.SendMessage(packet.Pack());

                // Recieve the data
                var data = client.Udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.NewAgendaEventAck:
                        // New event added correctly
                        client.DebugInfo("New Agenda event: Added new event!");
                        message = "New Agenda event added correctly.";
                        return true;
                    case Packets.PacketTypes.DeleteAgendaEventAck:
                        // Event deleted correctly
                        client.DebugInfo("Delete Agenda event: Deleted correctly!");
                        message = "Agenda event has been deleted";
                        return true;

                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        client.DebugInfo("Agenda event error: " + message);
                        break;
                    default:
                        client.DebugInfo("Agenda event error: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                client.DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }
    }
}
