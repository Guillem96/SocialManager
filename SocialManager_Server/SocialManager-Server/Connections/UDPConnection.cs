using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.Connections
{
    /// <summary>
    /// Manage an UDP connection. Sending and recieving messages from client
    /// </summary>
    class UDPConnection : Connection
    {
        private UdpClient socket;
        public UdpClient Socket { get => socket; set => socket = value; }


        public UDPConnection()
        {
            socket = new UdpClient(PortUDP);
        }

        /// <summary>
        /// Recieve message.
        /// </summary>
        /// <param name="address">The address is filled with the client info.</param>
        /// <returns></returns>
        public override byte[] RecieveMessage(ref IPEndPoint address)
        {
            return socket.Receive(ref address);
        }

        /// <summary>
        /// Send message to a client.
        /// </summary>
        /// <param name="msg">Bytes to send.</param>
        /// <param name="address">Destination address.</param>
        public override void SendMessage(byte[] msg, IPEndPoint address)
        {
            socket.Send(msg, msg.Length, address);
        }

        public override void SendError(string message, IPEndPoint address)
        {
            byte[] packet = new Packets.AckErrorPacket(Packets.PacketTypes.Error, message).Pack();
            SendMessage(packet, address);
        }
    }
}
