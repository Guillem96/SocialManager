using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.Connections
{
    class TCPConnection : Connection
    {
        private TcpListener listener;
        public TcpListener Listener { get => listener; set => listener = value; }


        public TCPConnection()
        {
            listener = new TcpListener(IPAddress.Parse(ServerIP), PortTCP);
            listener.Start();
        }

        /// <summary>
        /// Recieve message.
        /// </summary>
        /// <param name="Client">The client is filled with the accepted client.</param>
        /// <returns></returns>
        public byte[] RecieveMessage(ref TcpClient client)
        {
            client = listener.AcceptTcpClient();
            NetworkStream ns = client.GetStream();
            byte[] data = new byte[client.ReceiveBufferSize];
            ns.Read(data, 0, data.Length);
            return data;
        }

        /// <summary>
        /// Send message to a client.
        /// </summary>
        /// <param name="msg">Bytes to send.</param>
        /// <param name="Client">Destination client.</param>
        public void SendMessage(byte[] msg, TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            ns.Write(msg, 0, msg.Length);
        }

        public override void SendError(string message, IPEndPoint address)
        {
            byte[] packet = new Packets.AckErrorPacket(Packets.PacketTypes.Error, message).Pack();
            SendMessage(packet, address);
        }
    }
}
