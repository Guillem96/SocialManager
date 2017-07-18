﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client
{
    static class ClientController
    {
        public static Client client; //< Reference to current client

        public static bool Register(Profile newProfile, out string message)
        { 
            return client.Register(newProfile, out message);
        }

        public static bool Login(string username, string password, out string message)
        {
            return client.Login(username, password, out message);
        }

    }
}