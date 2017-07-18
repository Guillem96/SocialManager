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
            DebugInfo("UDP process started.");
            while (true)
            {
                // Recieve the packet; TODO: Read the port from file
                IPEndPoint tmp = new IPEndPoint(IPAddress.Any, 11000);
                var data = Udp.RecieveMessage(ref tmp);

                // New request
                DebugInfo("Crearting new thread to attend the Udp request.");
                Thread t = new Thread(() => UdpRequests(data, tmp));
                t.Start();
            }
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
                    UdpUtilitiesAccounts.Register(data, tmp, this);     
                    break;

                // Recieve Login request
                case Packets.PacketTypes.LoginReq:
                    UdpUtilitiesAccounts.Login(data, tmp, this);
                    break;

                // Recieve Logout Request
                case Packets.PacketTypes.LogoutReq:
                    UdpUtilitiesAccounts.Logout(data, tmp, this);
                    break;

                // Profile update request
                case Packets.PacketTypes.ProfileUpdateReq:
                    UdpUtilitiesAccounts.ProfileUpdate(data, tmp, this);
                    break;

                // Delete account request
                case Packets.PacketTypes.DeleteAccountReq:
                    UdpUtilitiesAccounts.DeleteAccount(data, tmp, this);
                    break;

                // Recieve Alive Inf
                case Packets.PacketTypes.AliveInf:
                    UdpUtilitiesAlive.Alive(data, tmp, this);
                    break;

                // Create new contact request
                case Packets.PacketTypes.NewContactReq:
                    UdpUtilitiesContacts.NewContactRequest(data, this, tmp);
                    break;

                case Packets.PacketTypes.ClientsQueryReq:
                    UdpUtilitiesContacts.ClientsQuery(data, this, tmp);
                    break;
                // Accept a contact request
                case Packets.PacketTypes.AcceptNewContact:
                    UdpUtilitiesContacts.AnswerContactRequest(data, this, tmp, true);
                    break;

                // Refuse a contact request
                case Packets.PacketTypes.RegNewContact:
                    UdpUtilitiesContacts.AnswerContactRequest(data, this, tmp, false);
                    break;

                // Contact requests list requested
                case Packets.PacketTypes.ListContactReq:
                    UdpUtilitiesContacts.SendContactRequests(data, this, tmp);
                    break;
            }
        }

        protected override void TCP()
        {
            return;
        }
    }
}
