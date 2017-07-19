using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client.Connections
{
    /// <summary>
    /// Declare the basic methods for TCP and UDP connections for the client
    /// </summary>
    abstract class Connection
    {
        public string ServerIP = "127.0.0.1";
        public int PortUDP = 11000;
        public int PortTCP = 5000;

        public abstract void SendMessage(byte[] msg);
        public abstract byte[] RecieveMessage();
    }
}
