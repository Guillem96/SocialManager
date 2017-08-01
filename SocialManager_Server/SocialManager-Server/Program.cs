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
            using(var db = new Models.ServerDatabase())
            {
                if (!db.DatabaseExists())
                {
                    db.CreateDatabase();
                    db.SocialNetworks.InsertOnSubmit(new Models.SocialNetwork()
                    {
                        Name = "Twitter"
                    });
                    db.SocialNetworks.InsertOnSubmit(new Models.SocialNetwork()
                    {
                        Name = "Instagram"
                    });
                    db.SubmitChanges();
                }
            }
            // Start server
            ServerLogic.Server server = new ServerLogic.Server("Social Manager server");
            server.MainLoop();
        }
    }
}
