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
                    UdpUtilities.Register(data, tmp, this);     
                    break;

                // Recieve Login request
                case Packets.PacketTypes.LoginReq:
                    UdpUtilities.Login(data, tmp, this);
                    break;

                // Recieve Alive Inf
                case Packets.PacketTypes.AliveInf:
                    UdpUtilities.Alive(data, tmp, this);
                    break;

                // Recieve Logout Request
                case Packets.PacketTypes.LogoutReq:
                    UdpUtilities.Logout(data, tmp, this);
                    break;

                case Packets.PacketTypes.DeleteAccountReq:
                    UdpUtilities.DeleteAccount(data, tmp, this);
                    break;
            }
        }

        protected override void TCP()
        {
            return;
        }
    }
}
