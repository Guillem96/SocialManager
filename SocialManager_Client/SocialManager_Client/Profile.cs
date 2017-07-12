using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client
{
    /// <summary>
    /// Client profile
    /// </summary>
    class Profile
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

        public int ClientID { get => clientID; set => clientID = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public int Age { get => age; set => age = value; }
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        internal Sex Genre { get => genre; set => genre = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
    }
}
