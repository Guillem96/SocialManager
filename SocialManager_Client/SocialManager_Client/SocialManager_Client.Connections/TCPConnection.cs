using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client.Connections
{
    class TCPConnection : Connection
    {
        private TcpClient client;
        private NetworkStream ns;

        public TcpClient Client { get => client; set => client = value; }


        public TCPConnection()
        {
            ReadServerData();
            Client = new TcpClient(ServerIP, PortTCP);
            ns = Client.GetStream();
        }

        /// <summary>
        /// Recieve message.
        /// </summary>
        /// <returns></returns>
        public override byte[] RecieveMessage()
        {
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = ns.Read(bytesToRead, 0, client.ReceiveBufferSize);
            ns.Flush();
            return bytesToRead;
        }

        /// <summary>
        /// Send message.
        /// </summary>
        /// <param name="msg">Bytes to send.</param>
        public override void SendMessage(byte[] msg)
        {
            ns.Write(msg, 0, msg.Length);
            ns.Flush();
        }
    }
}
