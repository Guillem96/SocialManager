using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SocialManager_Server
{
    class Program
    {
        private static void Main(String[] args)
        {

            // Start server
            ServerLogic.Server server = new ServerLogic.Server("Social Manager server");
            server.MainLoop();
        }
    }
}
