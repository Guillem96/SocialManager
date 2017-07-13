﻿using System;
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
        public abstract void SendMessage(byte[] msg);
        public abstract byte[] RecieveMessage();
    }
}