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
            ReadServerData();
            socket = new UdpClient(PortUDP);
            socket.Client.ReceiveTimeout = 2000;
        }

        /// <summary>
        /// Recieve message.
        /// </summary>
        /// <param name="address">The address is filled with the client info.</param>
        /// <returns></returns>
        public override byte[] RecieveMessage(ref IPEndPoint address)
        {
            try
            {
                return socket.Receive(ref address);

            }
            catch (SocketException ex)
            {
                if ((SocketError)ex.ErrorCode == SocketError.TimedOut)
                    return null;
                else
                    throw new Exception("Unexpected socket error");

            }
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
