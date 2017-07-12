using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.Models
{
    [Table(Name ="Clients")]
    class Client
    {
        internal enum Sex { Male, Female }

        private int clientID;
        private string firstName;
        private string lastName;
        private int age;
        private string phoneNumber;
        private Sex genre;
        private string username;
        private string password;

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int ClientID { get => clientID; set => clientID = value; }

        [Column]
        public string FirstName { get => firstName; set => firstName = value; }
        [Column]
        public string LastName { get => lastName; set => lastName = value; }
        [Column]
        public string Username { get => username; set => username = value; }
        [Column]
        public string Password { get => password; set => password = value; }
    }
}
