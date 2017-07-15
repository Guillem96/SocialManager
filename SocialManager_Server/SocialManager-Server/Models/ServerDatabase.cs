using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.Models
{
    /// <summary>
    /// The database.
    /// </summary>
    class ServerDatabase : DataContext
    {
        // Clients table
        public Table<Client> Clients;
        public Table<Contact> Contacts;


        public ServerDatabase() : base(@"Data Source=.\SQLEXPRESS;
                                        AttachDbFilename=" +
                                        System.IO.Directory.GetCurrentDirectory()+ @"\ServerDB.MDF;
                                        Integrated Security=True;
                                        Connect Timeout=30;
                                        User Instance=True")
        { }
    }
}
