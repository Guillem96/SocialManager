using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SocialManager_Server.Models;

namespace SocialManager_Server.ServerLogic
{
    class Server : SuperServer
    {
        public Server(string name) : base(name)
        {
        }

        /// <summary>
        /// Used for accepting connections and registering clients
        /// </summary>
        protected override void UDP()
        {
            try
            {
                DebugInfo("UDP process started.");
                while (serverUp)
                {
                    // Recieve the packet; TODO: Read the port from file
                    IPEndPoint tmp = new IPEndPoint(IPAddress.Any, Udp.PortUDP);
                    var data = Udp.RecieveMessage(ref tmp);

                    if(data != null)
                    {
                        // New request
                        DebugInfo("Crearting new thread to attend the Udp request.");
                        Thread t = new Thread(() => UdpRequests(data, tmp));
                        t.Start();
                    }
                }
            }
            catch (Exception e)
            {
                DebugInfo("Unexpected error UDP: "+  e.ToString());
            }
            DebugInfo("UDP Service is down.");
        }

        private void UdpRequests(byte[] data, IPEndPoint tmp)
        {
            // Read the type of the packet
            var packet = Packets.Packet.Unpack<Packets.Packet>(data);

            // Depending on the type extract the remaning data
            switch ((Packets.PacketTypes)packet.Type)
            {
                // Recieve a register request
                case Packets.PacketTypes.RegisterReq:
                    UdpUtilities.Register(data, tmp, this);     
                    break;

                // Recieve Login request
                case Packets.PacketTypes.LoginReq:
                    UdpUtilities.Login(data, tmp, this);
                    break;

                // Recieve Logout Request
                case Packets.PacketTypes.LogoutReq:
                    UdpUtilities.Logout(data, tmp, this);
                    break;

                // Profile update request
                case Packets.PacketTypes.ProfileUpdateReq:
                    UdpUtilities.ProfileUpdate(data, tmp, this);
                    break;

                // Delete account request
                case Packets.PacketTypes.DeleteAccountReq:
                    UdpUtilities.DeleteAccount(data, tmp, this);
                    break;

                // Recieve Alive Inf
                case Packets.PacketTypes.AliveInf:
                    UdpUtilities.Alive(data, tmp, this);
                    break;

                // Create new contact request
                case Packets.PacketTypes.NewContactReq:
                    UdpUtilities.NewContactRequest(data, this, tmp);
                    break;

                case Packets.PacketTypes.ClientsQueryReq:
                    UdpUtilities.ClientsQuery(data, this, tmp);
                    break;
                // Accept a contact request
                case Packets.PacketTypes.AcceptNewContact:
                    UdpUtilities.AnswerContactRequest(data, this, tmp, true);
                    break;

                // Refuse a contact request
                case Packets.PacketTypes.RegNewContact:
                    UdpUtilities.AnswerContactRequest(data, this, tmp, false);
                    break;

                // Contact requests list requested
                case Packets.PacketTypes.ListContactReq:
                    UdpUtilities.SendContactRequests(data, this, tmp);
                    break;

                // New agenda event
                case Packets.PacketTypes.NewAgendaEventReq:
                    UdpUtilities.AgendaEvent(data, this, false, tmp);
                    break;

                // Delete agenda event
                case Packets.PacketTypes.DeleteAgendaEventReq:
                    UdpUtilities.AgendaEvent(data, this, true, tmp);
                    break;

                // Link social network
                case Packets.PacketTypes.LinkSocialNetworkReq:
                    UdpUtilities.LinkSocialNet(data, tmp, this);
                    break;

                // Delete Link social network
                case Packets.PacketTypes.DeleteLinkSocialNetReq:
                    UdpUtilities.DeleteLinkSocialNet(data, tmp, this);
                    break;
            }
        }

        protected override void TCP()
        {
            try
            {
                Tcp = new Connections.TCPConnection();
                DebugInfo("TCP process has started.");

                while (serverUp)
                {
                    // Recieve the message
                    TcpClient client = null;
                    var data = Tcp.RecieveMessage(ref client);

                    if (data != null)
                    {
                        // New request
                        DebugInfo("Crearting new thread to attend the Tcp request.");
                        Thread t = new Thread(() => TcpRequests(data, client));
                        t.Start();
                    }
                }
            }
            catch (Exception e)
            {
                DebugInfo("Unexpected error TCP: " + e.ToString());
            }
            DebugInfo("TCP Service is down.");
        }

        private void TcpRequests(byte[] data, TcpClient client)
        {
            // Read the type of the packet
            var packet = Packets.Packet.Unpack<Packets.Packet>(data);

            switch ((Packets.PacketTypes)packet.type)
            {
                case Packets.PacketTypes.ReadyChatReq:
                    TcpUtilities.ReadyChatClient(data, client, this);
                    break;
                case Packets.PacketTypes.SendMessageReq:
                    TcpUtilities.SendMessage(data, client, this);
                    break;
            }
        }
    }
}
