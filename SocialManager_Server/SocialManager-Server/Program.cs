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
                //db.DeleteDatabase();
                //db.CreateDatabase();
                //db.AgendaEvents.InsertOnSubmit(new Models.AgendaEvent()
                //{
                //    Client = db.Clients.Single(c => c.Username == "Guillem96"),
                //    EventInfo = "Cena viernes noche",
                //    Date = DateTime.Now,
                //    EventName = "Tagliattela."
                //});
                //db.SubmitChanges();
            }
            // Start server
            ServerLogic.Server server = new ServerLogic.Server("Social Manager server");
            server.MainLoop();
        }
    }
}
