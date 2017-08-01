using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client
{
    static partial class UdpUtilities
    {
        public static bool ClientQuery(string query, ref List<Profile> profiles, Client client, out string message)
        {
            try
            {
                // Prepare the packet to send
                Packets.ClientQueryPacket packet = new Packets.ClientQueryPacket(
                                                        Packets.PacketTypes.ClientsQueryReq,
                                                        client.Alea,
                                                        client.Profile.Username,
                                                        query
                                                    );

                // send the package
                client.Udp.SendMessage(packet.Pack());

                // Recieve packet
                byte[] data = client.Udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.ClientsQueryAck:
                        // Update contact requests list
                        Packets.ClientQueryPacket queryResult =
                                                    Packets.Packet.Unpack<Packets.ClientQueryPacket>(data);


                        profiles = queryResult.Profiles;

                        message = "Recieved list correctly";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        client.DebugInfo("Client query list: " + message);
                        break;
                    default:
                        client.DebugInfo("Client query list: Unexpected type.");
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

        public static bool GetContactRequests(Client client, out string message)
        {
            try
            {
                // Prepare the packet to send
                Packets.BasicReqPacket packet = new Packets.BasicReqPacket(
                                                        Packets.PacketTypes.ListContactReq,
                                                        client.Alea,
                                                        client.Profile.Username
                                                    );

                // send the package
                client.Udp.SendMessage(packet.Pack());

                // Recieve packet
                byte[] data = client.Udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.ContactAck:
                        // Update contact requests list
                        Packets.ContactReqListPacket contactsRequests =
                                                    Packets.Packet.Unpack<Packets.ContactReqListPacket>(data);

                        client.Profile.RecievedContactRequests = contactsRequests.Recieved;
                        client.Profile.SentContactRequests = contactsRequests.Sent;

                        message = "Recieved list correctly";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        client.DebugInfo("Contact request list: " + message);
                        break;
                    default:
                        client.DebugInfo("Contact request list: Unexpected type.");
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

        public static bool AnswerContactRequest(ContactRequest req, bool ack, Client client, out string message)
        {
            try
            {
                // Prepare the packet to send
                Packets.ContactReqPacket packet = new Packets.ContactReqPacket(
                                                        (ack) ? Packets.PacketTypes.AcceptNewContact : Packets.PacketTypes.RegNewContact,
                                                        client.Alea,
                                                        req.From.Username,
                                                       client.Profile.Username
                                                    );

                // send the package
                client.Udp.SendMessage(packet.Pack());

                // Recieve packet
                byte[] data = client.Udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.ContactAck:
                        if (ack)
                        {
                            client.DebugInfo("Answer Contact Request: Accepted " + req.From.Username + " correctly.");
                        }
                        else
                        {
                            client.DebugInfo("Answer Contact Request: Refused " + req.From.Username + " correctly.");
                        }

                        // Update the request list
                        client.GetContactRequestList(out message);

                        message = "Answered correctly";
                        return true;

                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        client.DebugInfo("Answer Contact Request: " + message);
                        break;
                    default:
                        client.DebugInfo("Answer Contact Request: Unexpected type.");
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

        public static bool SendContactRequest(string to, Client client, out string message)
        {
            try
            {
                // Prepare the packet to send
                Packets.ContactReqPacket packet = new Packets.ContactReqPacket(
                                                        Packets.PacketTypes.NewContactReq,
                                                        client.Alea,
                                                        client.Profile.Username,
                                                        to
                                                    );

                // send the package
                client.Udp.SendMessage(packet.Pack());

                // Recieve packet
                byte[] data = client.Udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.ContactAck:
                        client.DebugInfo("Send Contact request: Sent succesful to " + to);

                        // Update the request list
                        client.GetContactRequestList(out message);

                        message = "Request sent correctly.";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        client.DebugInfo("Send Contact request: " + message);
                        break;
                    default:
                        client.DebugInfo("Send Contact request: Unexpected type.");
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
