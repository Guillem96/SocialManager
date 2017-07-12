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
            // Testing database
            Models.ServerDatabase db = new Models.ServerDatabase();

            if (db.DatabaseExists() == false)
            {
                db.CreateDatabase();
            }

            db.Clients.InsertOnSubmit(new Models.Client() { FirstName = "Ester" });
            db.Clients.InsertOnSubmit(new Models.Client() { FirstName = "Guillem" });

            db.SubmitChanges();

            foreach (Models.Client c in db.Clients)
            {
                Console.WriteLine(c.ClientID + " " + c.FirstName);
            }

            Server server = new Server("Social Manager Server");
            server.MainLoop();
        }
    }
}
