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
        public Table<ContactRequest> ContactRequests;
        public Table<Message> Messages;
        public Table<AgendaEvent> AgendaEvents;
        public Table<SocialNetwork> SocialNetworks;
        public Table<LinkedSocialNetwork> LinkedSocialNetworks;


        public ServerDatabase() : base(@"Data Source=.\SQLEXPRESS;
                                        AttachDbFilename=" + Environment.CurrentDirectory + @"\ServerDataBase.MDF;
                                        Integrated Security=True;
                                        Connect Timeout=30;
                                        User Instance=True")
        { }
    }
}
