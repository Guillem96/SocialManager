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
            using (var db = new Models.ServerDatabase())
            {
                //db.ContactRequests.InsertOnSubmit(new Models.ContactRequest()
                //{
                //    From = db.Clients.Single(c => c.Username == "Guillem96"),
                //    To = db.Clients.Single(c => c.Username == "Ester96")
                //});

                //db.ContactRequests.InsertOnSubmit(new Models.ContactRequest()
                //{
                //    From = db.Clients.Single(c => c.Username == "Peludet96"),
                //    To = db.Clients.Single(c => c.Username == "Ester96")
                //});

                //db.ContactRequests.InsertOnSubmit(new Models.ContactRequest()
                //{
                //    From = db.Clients.Single(c => c.Username == "Sora96"),
                //    To = db.Clients.Single(c => c.Username == "Ester96")
                //});

                //db.SubmitChanges();

                foreach (var r in db.ContactRequests)
                {
                    Console.WriteLine(r.ToString());
                }
            }
            // Start server
            ServerLogic.Server server = new ServerLogic.Server("Social Manager server");
            server.MainLoop();
        }
    }
}
