using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            listener.Server.ReceiveTimeout = 1000;
            listener.Start();
        }

        /// <summary>
        /// Recieve message.
        /// </summary>
        /// <param name="Client">The client is filled with the accepted client.</param>
        /// <returns></returns>
        public byte[] RecieveMessage(ref TcpClient client)
        {
            if (listener.Pending())
            {
                client = listener.AcceptTcpClient();
                if(client != null)
                {
                    NetworkStream ns = client.GetStream();
                    byte[] data = new byte[client.ReceiveBufferSize];
                    ns.Read(data, 0, data.Length);
                    return data;
                }
            }
            else //< 2 Seconds timeout recieve
                Thread.Sleep(2000);
            
            return null;
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

        public void SendError(string message, TcpClient client)
        {
            byte[] packet = new Packets.AckErrorPacket(Packets.PacketTypes.Error, message).Pack();
            SendMessage(packet, client);
        }
    }
}
