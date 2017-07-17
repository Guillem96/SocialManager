using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.Models
{
    [Table(Name = "Contacts")]
    class Contact
    {
        private int contactID;
        [Column(Name ="Client1")]
        private int client1ID;
        [Column(Name = "Client2")]
        private int client2ID;

        private EntityRef<Client> client1 = new EntityRef<Client>();
        private EntityRef<Client> client2 = new EntityRef<Client>();

        [Association(Name = "FK_1", Storage = "client1", IsForeignKey = true, ThisKey = "client1ID", OtherKey = "ClientID")]
        public Client Client1 { get => client1.Entity; set => client1.Entity = value; }

        [Association(Name = "FK_2", Storage = "client2", IsForeignKey = true, ThisKey = "client2ID", OtherKey = "ClientID")]
        public Client Client2 { get => client2.Entity; set => client2.Entity = value; }

        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int ContactID { get => contactID; set => contactID = value; }

        public override string ToString()
        {
            return Client1.Username + " - " + Client2.Username;
        }
    }
}
