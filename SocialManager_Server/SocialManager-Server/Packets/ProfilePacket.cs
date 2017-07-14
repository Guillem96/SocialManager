using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SocialManager_Server.Models;
using System.Data.Linq;

namespace SocialManager_Server.Packets
{
    /// <summary>
    /// Package sended by the client in order to register
    /// </summary>
    [SerializableAttribute]
    [XmlRoot(ElementName = "Packet")]
    public class ProfilePacket : Packet
    {
        private string firstName;
        private string lastName;
        private int age;
        private string phoneNumber;
        private Models.Client.Sex genre;
        private string username;
        private string password;
        private string email;
        private List<Client> contacts;


        [XmlElement(ElementName = "FirsName")]
        public string FirstName { get => firstName; set => firstName = value; }
        [XmlElement(ElementName = "LastName")]
        public string LastName { get => lastName; set => lastName = value; }
        [XmlElement(ElementName = "Age")]
        public int Age { get => age; set => age = value; }
        [XmlElement(ElementName = "PhoneNumber")]
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        [XmlElement(ElementName = "Genre")]
        internal Client.Sex Genre { get => genre; set => genre = value; }
        [XmlElement(ElementName = "Username")]
        public string Username { get => username; set => username = value; }
        [XmlElement(ElementName = "Password")]
        public string Password { get => password; set => password = value; }
        [XmlElement(ElementName = "Email")]
        public string Email { get => email; set => email = value; }
        [XmlArray("Contacts")]
        [XmlArrayItem("ContactsItem")]
        public List<Client> Contacts { get => contacts; set => contacts = value; }

        public ProfilePacket() : base() { }

        internal ProfilePacket(PacketTypes type, 
                                    string alea,
                                    string firstName, 
                                    string lastName,
                                    int age,
                                    string phoneNumber,
                                    Client.Sex genre,
                                    string username,
                                    string password,
                                    List<Client> contacts
                                    ) : base(type, alea)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            PhoneNumber = phoneNumber;
            Genre = genre;
            Username = username;
            Password = password;
            Contacts = contacts;
        }

        public override string ToString()
        {
            return base.ToString() + String.Format(@", FirstName={0}, LastName={1}, Age={2}, PhoneNumber={3}
                                       , Genre={4}, Email={5}, Username={6}, Password={7}]",
                                       FirstName, LastName, Age, PhoneNumber, Genre.ToString(), Email,
                                       Username, Password);
        }
    }
}
