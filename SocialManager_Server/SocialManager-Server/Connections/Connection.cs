using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.Connections
{
    /// <summary>
    /// Declare the basic methods for TCP and UDP connections
    /// </summary>
    abstract class Connection
    {
        public string ServerIP = "127.0.0.1";
        public int PortUDP = 11000;
        public int PortTCP = 5000;

        public virtual void SendMessage(byte[] msg, IPEndPoint address) { }
        public virtual byte[] RecieveMessage(ref IPEndPoint address) { return null; }
        public virtual void SendError(string message, IPEndPoint addresss) { }
    }
}
