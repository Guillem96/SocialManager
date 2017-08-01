using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client.ClientLogic
{
    [MetadataType(typeof(Client))]
    partial class Client
    {

        public bool GetContactRequestList(out string message)
        {
            try
            {
                // Prepare the packet to send
                Packets.BasicReqPacket packet = new Packets.BasicReqPacket(
                                                        Packets.PacketTypes.ListContactReq,
                                                        alea,
                                                        profile.Username
                                                    );

                // send the package
                udp.SendMessage(packet.Pack());

                // Recieve packet
                byte[] data = udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.ContactAck:
                        // Update contact requests list
                        Packets.ContactReqListPacket contactsRequests =
                                                    Packets.Packet.Unpack<Packets.ContactReqListPacket>(data);

                        profile.RecievedContactRequests = contactsRequests.Recieved;
                        profile.SentContactRequests = contactsRequests.Sent;

                        message = "Recieved list correctly";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        DebugInfo("Contact request list: " + message);
                        break;
                    default:
                        DebugInfo("Contact request list: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }

        public bool SendContactRequest(string to, out string message)
        {
            try
            {
                // Prepare the packet to send
                Packets.ContactReqPacket packet = new Packets.ContactReqPacket(
                                                        Packets.PacketTypes.NewContactReq,
                                                        alea,
                                                        profile.Username,
                                                        to
                                                    );

                // send the package
                udp.SendMessage(packet.Pack());

                // Recieve packet
                byte[] data = udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.ContactAck:
                        DebugInfo("Send Contact request: Sent succesful to " + to);

                        // Update the request list
                        GetContactRequestList(out message);

                        message = "Request sent correctly.";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        DebugInfo("Send Contact request: " + message);
                        break;
                    default:
                        DebugInfo("Send Contact request: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }

        public bool AnswerContactRequest(ContactRequest req, bool ack, out string message)
        {
            try
            {
                // Prepare the packet to send
                Packets.ContactReqPacket packet = new Packets.ContactReqPacket(
                                                        (ack) ? Packets.PacketTypes.AcceptNewContact : Packets.PacketTypes.RegNewContact,
                                                        alea,
                                                        req.From.Username,
                                                        profile.Username
                                                    );

                // send the package
                udp.SendMessage(packet.Pack());

                // Recieve packet
                byte[] data = udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.ContactAck:
                        if (ack)
                        {
                            // Todo: Get Contacts info
                            DebugInfo("Answer Contact Request: Accepted " + req.From.Username + " correctly.");
                        }
                        else
                        {
                            DebugInfo("Answer Contact Request: Refused " + req.From.Username + " correctly.");
                        }

                        // Update the request list
                        GetContactRequestList(out message);

                        message = "Answered correctly";
                        return true;

                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        DebugInfo("Answer Contact Request: " + message);
                        break;
                    default:
                        DebugInfo("Answer Contact Request: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }
    }
}
