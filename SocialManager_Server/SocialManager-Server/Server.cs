using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SocialManager_Server.Models;

namespace SocialManager_Server
{
    class Server : SuperSever
    {
        public Server(string name) : base(name)
        {
        }

        /// <summary>
        /// Used for accepting connections and registering clients
        /// </summary>
        protected override void UDP()
        {
            DebugInfo("UDP connection started.");
            while (true)
            {
                // Recieve the packet; TODO: Read the port from file
                DebugInfo("Waiting UDP requests.");
                IPEndPoint tmp = new IPEndPoint(IPAddress.Any, 11000);
                var data = udp.RecieveMessage(ref tmp);

                // New request
                DebugInfo("Crearting new thread.");
                Thread t = new Thread(() => UdpRequests(data, tmp));
                t.Start();
            }
        }

        private void UdpRequests(byte[] data, IPEndPoint tmp)
        {
            // Read the type of the packet
            var packet = Packets.Packet.Unpack<Packets.Packet>(data);

            // Client who has send the request
            Client current = null;
            string message = "";

            // Depending on the type extract the remaning data
            switch ((Packets.PacketTypes)packet.Type)
            {
                // Recieve a register request
                case Packets.PacketTypes.RegisterReq:
                    Packets.ProfilePacket regPacket = Packets.Packet.Unpack<Packets.ProfilePacket>(data);
                    Console.WriteLine(Encoding.ASCII.GetString(data));
                    // Send a package depending on registration success
                    if (ClientsManagement.RegisterClient(regPacket, tmp, out message))
                    {
                        udp.SendMessage(new Packets.AckErrorPacket(Packets.PacketTypes.RegisterAck,
                                                                    "Congratulations now you are registered.").Pack(), tmp);
                        DebugInfo(regPacket.FirstName + " " + regPacket.LastName + " has been registered as " + regPacket.Username);
                    }
                    else
                    {
                        udp.SendError(message, tmp);
                    }
                    break;

                // Recieve Login request
                case Packets.PacketTypes.LoginReq:
                    Packets.LoginReqPacket logPacket = Packets.Packet.Unpack<Packets.LoginReqPacket>(data);
                    if(ClientsManagement.LoginClient(logPacket, ref current, out message))
                    {
                        // Client login 
                        DebugInfo("Client " + current.ToString() + " is now logged in.");
                        string alea = GenerateAlea();
                        ChangeStatus(logPacket.Username, ClientStatus.Status.Logged, alea);
                        // Send the profile info to client
                        DebugInfo("Sending profile info to " + logPacket.Username + ".");
                        udp.SendMessage(new Packets.ProfilePacket(
                                                    Packets.PacketTypes.LoginAck,
                                                    alea, // New alea generated
                                                    current.FirstName,
                                                    current.LastName,
                                                    current.Age,
                                                    current.PhoneNumber,
                                                    current.Genre,
                                                    current.Username,
                                                    current.Password).Pack(), tmp);
                    }
                    else
                    {
                        udp.SendError(message, tmp);
                    }
                    break;
            }
        }

        protected override void TCP()
        {
            return;
        }
    }
}
