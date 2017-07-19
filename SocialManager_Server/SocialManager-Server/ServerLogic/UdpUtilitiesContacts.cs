using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SocialManager_Server.Packets;
using SocialManager_Server.ClientLogic;

namespace SocialManager_Server.ServerLogic
{
    static class UdpUtilitiesContacts
    {
        public static void ClientsQuery(byte[] data, Server server, IPEndPoint ip)
        {
            string message = "";

            // Unpack the petition
            Packets.ClientQueryPacket cPacket = Packet.Unpack<ClientQueryPacket>(data);

            List<Models.Client> queryResult = null;

            server.DebugInfo("Client query  list requested by " + cPacket.Username);
            server.DebugInfo(cPacket.ToString());

            if (ClientsManagement.GetClientsQueryResult(cPacket, server.GetClient(cPacket.Username),
                                                        ref queryResult, cPacket.Query, out message))
            {

                // List filled correctly
                server.Udp.SendMessage(new ClientQueryPacket(PacketTypes.ClientsQueryAck,
                                                                        cPacket.Alea,
                                                                        queryResult).Pack(),
                                                                        ip);

                server.DebugInfo("Client list query requests sended correctly to + " + cPacket.Username);
            }
            else
            {
                server.DebugInfo("Client list query requests error: " + message);
                server.Udp.SendError(message, ip);
            }
        }

        public static void SendContactRequests(byte[] data, Server server, IPEndPoint ip)
        {
            string message = "";

            // Unpack the petition
            BasicReqPacket cPacket = Packet.Unpack<BasicReqPacket>(data);

            List<Models.ContactRequest> sent = null;
            List<Models.ContactRequest> recieved = null;


            server.DebugInfo("Contact requests list requested by " + cPacket.Username);
            server.DebugInfo(cPacket.ToString());

            if (ClientsManagement.ContactRequestsList(cPacket,
                                                        server.GetClient(cPacket.Username),
                                                        ref sent,
                                                        ref recieved,
                                                        out message))
            {

                // List filled correctly
                server.Udp.SendMessage(new ContactReqListPacket(PacketTypes.ContactAck,
                                                                        cPacket.Alea,
                                                                        sent,
                                                                        recieved).Pack(),
                                                                        ip);

                server.DebugInfo("Contact list requests sended correctly to + " + cPacket.Username);
            }
            else
            {
                server.DebugInfo("Contact requests list error: " + message);
                server.Udp.SendError(message, ip);
            }
        }

        public static void NewContactRequest(byte[] data, Server server, IPEndPoint ip)
        {
            string message = "";

            // Unpack the petition
            ContactReqPacket cPacket = Packet.Unpack<ContactReqPacket>(data);

            ClientStatus current = server.GetClient(cPacket.From);

            if (ClientsManagement.NewContactRequest(cPacket, current, out message))
            {
                server.DebugInfo(cPacket.From + " made a contact request to " + cPacket.To);

                // Send the ack
                server.Udp.SendMessage(new AckErrorPacket(PacketTypes.ContactAck, "New contact request added to database").Pack(), ip);
            }
            else
            {
                server.DebugInfo("New Contact Request: Incorrect contact request.");
                // Send error
                server.Udp.SendError(message, ip);
            }
        }

        public static void AnswerContactRequest(byte[] data, Server server, IPEndPoint ip, bool ack)
        {
            string message = "";

            // Unpack the petition
            ContactReqPacket cPacket = Packet.Unpack<ContactReqPacket>(data);

            ClientStatus current = server.GetClient(cPacket.To);

            if (ClientsManagement.AckOrRegContactReq(cPacket, current, ack, out message))
            {
                server.DebugInfo("Answer Contact request: " + (ack
                                                                ? "Request accepted correctly."
                                                                : "Request refused correctly."));

                if (ack)
                    server.DebugInfo("New Contact added: " + cPacket.From + " - " + cPacket.To);

                // Send the ack
                server.Udp.SendMessage(new AckErrorPacket(PacketTypes.ContactAck, "Answered correctly").Pack(), ip);
            }
            else
            {
                server.DebugInfo("Answer Contact request: Incorrect contact request.");
                // Send error
                server.Udp.SendError(message, ip);
            }
        }
    }
}
