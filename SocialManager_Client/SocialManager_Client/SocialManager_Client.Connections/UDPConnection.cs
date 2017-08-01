using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client.Connections
{
    /// <summary>
    /// Manage an UDP connection. Sending and recieving messages from client
    /// </summary>
    class UDPConnection : Connection
    {
        private UdpClient socket;
        private IPEndPoint serverEndPoint;

        public UdpClient Socket { get => socket; set => socket = value; }
        public IPEndPoint ServerEndPoint { get => serverEndPoint; set => serverEndPoint = value; }

        public UDPConnection()
        {
            ReadServerData();
            socket = new UdpClient();
            ServerEndPoint = new IPEndPoint(IPAddress.Parse(ServerIP), PortUDP);
            socket.Connect(ServerEndPoint);
        }

        /// <summary>
        /// Recieve message.
        /// </summary>
        /// <param name="address">The address is filled with the client info.</param>
        /// <returns></returns>
        public override byte[] RecieveMessage()
        {
            return socket.Receive(ref serverEndPoint);
        }

        /// <summary>
        /// Send message to a client.
        /// </summary>
        /// <param name="msg">Bytes to send.</param>
        /// <param name="address">Destination address.</param>
        public override void SendMessage(byte[] msg)
        {
            socket.Send(msg, msg.Length);
        }
    }
}
