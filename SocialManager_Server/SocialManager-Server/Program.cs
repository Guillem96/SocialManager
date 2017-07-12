using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server
{
    class Program
    {
        private static void Main(String[] args)
        {
            foreach(Models.Client c in new Models.ServerDatabase().Clients)
            {
                Console.WriteLine(c.FirstName);
            }
            Server server = new Server("Social Manager Server");
            server.MainLoop();
        }
    }
}
