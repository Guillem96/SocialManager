using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SocialManager_Server.Models
{
    /// <summary>
    /// Database models. Stores client information.
    /// </summary>
    [Table(Name ="Clients")]
    class Client
    {
        internal enum Sex { Male, Female }

        private int clientID;
        private string firstName;
        private string lastName;
        private int age;
        private string phoneNumber;
        private string email;
        private Sex genre;
        private string username;
        private string password;
        private byte[] ip; 

        // Define columns
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int ClientID { get => clientID; set => clientID = value; }
        [Column(CanBeNull = false)]
        public string FirstName { get => firstName; set => firstName = value; }
        [Column(CanBeNull = false)]
        public string LastName { get => lastName; set => lastName = value; }
        [Column(CanBeNull = false)]
        public string Username { get => username; set => username = value; }
        [Column(CanBeNull = false)]
        public string Password { get => password; set => password = value; }
        [Column(CanBeNull = false)]
        public int Age { get => age; set => age = value; }
        [Column(CanBeNull = false)]
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        [Column(CanBeNull = false)]
        internal Sex Genre { get => genre; set => genre = value; }
        [Column(CanBeNull = false)]
        public string Email { get => email; set => email = value; }
        [Column(CanBeNull = false)]
        public byte[] Ip { get => ip; set => ip = value; }

        public override string ToString()
        {
            return Username + " - " + FirstName + " " + LastName;
        }
    }
}
