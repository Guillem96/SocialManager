using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Client.Packets
{
    /// <summary>
    /// Type of packet that is send in order to register
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Packet")]
    public class ProfilePacket : Packet
    {
        private string firstName;
        private string lastName;
        private int age;
        private string phoneNumber;
        private Profile.Sex genre;
        private string username;
        private string password;
        private string email;
        private List<Contact> contacts;

        [XmlElement(ElementName = "FirsName")]
        public string FirstName { get => firstName; set => firstName = value; }
        [XmlElement(ElementName = "LastName")]
        public string LastName { get => lastName; set => lastName = value; }
        [XmlElement(ElementName = "Age")]
        public int Age { get => age; set => age = value; }
        [XmlElement(ElementName = "PhoneNumber")]
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        [XmlElement(ElementName = "Gender")]
        internal Profile.Sex Genre { get => genre; set => genre = value; }
        [XmlElement(ElementName = "Username")]
        public string Username { get => username; set => username = value; }
        [XmlElement(ElementName = "Password")]
        public string Password { get => password; set => password = value; }
        [XmlElement(ElementName = "Email")]
        public string Email { get => email; set => email = value; }
        [XmlArray("Contacts")]
        [XmlArrayItem("ContactsItem")]
        public List<Contact> Contacts { get => contacts; set => contacts = value; }

        public ProfilePacket() : base()
        {

        }

        internal ProfilePacket(PacketTypes type,string alea, Profile p) : base(type, alea)
        {
            FirstName = p.FirstName;
            LastName = p.LastName;
            Age = p.Age;
            PhoneNumber = p.PhoneNumber;
            Genre = p.Gender;
            Username = p.Username;
            Password = p.Password;
            Email = p.Email;
            Contacts = p.Contacts;
        }

        internal ProfilePacket(PacketTypes type,
                                    string alea,
                                    string firstName, 
                                    string lastName,
                                    int age,
                                    string phoneNumber,
                                    Profile.Sex genre,
                                    string username,
                                    string password,
                                    string email) : base(type, alea)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            PhoneNumber = phoneNumber;
            Genre = genre;
            Username = username;
            Password = password;
            Email = email;
        }

        public override string ToString()
        {
            return base.ToString() + String.Format(@", FirstName={0}, LastName={1}, Age={2}, PhoneNumber={3}
                                       , Gender={4}, Email={5}, Username={6}, Password={7}]",
                                       FirstName, LastName, Age, PhoneNumber, Genre.ToString(), Email,
                                       Username, Password);
        }
    }
}
