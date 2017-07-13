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
        public abstract void SendMessage(byte[] msg, IPEndPoint address);
        public abstract byte[] RecieveMessage(ref IPEndPoint address);
        public abstract void SendError(string message, IPEndPoint addresss);
    }
}
