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
        public string ServerIP;
        public int PortUDP;
        public int PortTCP;

        public void ReadServerData()
        {
            string[] lines = System.IO.File.ReadAllLines(@"server.cfg");

            char[] spliter = new char[] { ':' };
            char[] whiteSpace = new char[] { ' ' };

            // Server ip
            ServerIP = lines[0].Split(spliter).Select(s => s.Trim(whiteSpace)).ToArray()[1];
            // Port UDP
            PortUDP = int.Parse(lines[1].Split(spliter).Select(s => s.Trim(whiteSpace)).ToArray()[1]);
            // Port TCP
            PortTCP = int.Parse(lines[2].Split(spliter).Select(s => s.Trim(whiteSpace)).ToArray()[1]);

        }

        public virtual void SendMessage(byte[] msg, IPEndPoint address) { }
        public virtual byte[] RecieveMessage(ref IPEndPoint address) { return null; }
        public virtual void SendError(string message, IPEndPoint addresss) { }
    }
}
