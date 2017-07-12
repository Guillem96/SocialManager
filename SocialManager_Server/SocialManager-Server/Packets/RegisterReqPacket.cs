using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SocialManager_Server.Models;

namespace SocialManager_Server.Packets
{
    [SerializableAttribute]
    [XmlRoot(ElementName = "Packet")]
    public class RegisterReqPacket : Packet
    {
        private string firstName;
        private string lastName;
        private int age;
        private string phoneNumber;
        private Models.Client.Sex genre;
        private string username;
        private string password;
        private string email;

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

        public RegisterReqPacket() : base() { }

        internal RegisterReqPacket(PacketTypes type, 
                                    string alea,
                                    string firstName, 
                                    string lastName,
                                    int age,
                                    string phoneNumber,
                                    Client.Sex genre,
                                    string username,
                                    string password) : base(type, alea)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
            this.PhoneNumber = phoneNumber;
            this.Genre = genre;
            this.Username = username;
            this.Password = password;
        }

        public override byte[] Pack()
        {
            return Encoding.ASCII.GetBytes(this.XmlSerializeToString());
        }

        public static new RegisterReqPacket Unpack(byte[] bytes)
        {
            return XmlUtilities.XmlDeserializeFromString<RegisterReqPacket>
                                                            (Encoding.ASCII.GetString(bytes));
        }

        public override string ToString()
        {
            return base.ToString() + Environment.NewLine +
                    "First name: " + FirstName + Environment.NewLine +
                    "Last name: " + LastName + Environment.NewLine +
                    "Age: " + Age + Environment.NewLine +
                    "Phone number: " + PhoneNumber + Environment.NewLine +
                    "Genre: " + Genre.ToString() + Environment.NewLine +
                    "Email: " + Email + Environment.NewLine +
                    "Username: " + Username + Environment.NewLine +
                    "Password: " + Password;
        }
    }
}
