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
    [Serializable]
    [XmlRoot(ElementName = "Packet")]
    public class ProfilePacket : Packet
    {
        private string firstName;
        private string lastName;
        private int age;
        private string phoneNumber;
        private Client.Sex genre;
        private string username;
        private string password;
        private string email;
        private List<ClientLogic.ClientStatus> contacts;
        private List<Message> messages;
        private List<AgendaEvent> agendaEvents;
        private List<LinkedSocialNetwork> socialNets;

        [XmlElement(ElementName = "FirsName")]
        public string FirstName { get => firstName; set => firstName = value; }
        [XmlElement(ElementName = "LastName")]
        public string LastName { get => lastName; set => lastName = value; }
        [XmlElement(ElementName = "Age")]
        public int Age { get => age; set => age = value; }
        [XmlElement(ElementName = "PhoneNumber")]
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        [XmlElement(ElementName = "Gender")]
        public Client.Sex Gender { get => genre; set => genre = value; }
        [XmlElement(ElementName = "Username")]
        public string Username { get => username; set => username = value; }
        [XmlElement(ElementName = "Password")]
        public string Password { get => password; set => password = value; }
        [XmlElement(ElementName = "Email")]
        public string Email { get => email; set => email = value; }
        [XmlArray("Contacts")]
        [XmlArrayItem("ContactsItem")]
        public List<ClientLogic.ClientStatus> Contacts { get => contacts; set => contacts = value; }
        [XmlArray("Messages")]
        [XmlArrayItem("MessageItem")]
        public List<Message> Messages { get => messages; set => messages = value; }
        [XmlArray("Agenda")]
        [XmlArrayItem("Event")]
        public List<AgendaEvent> AgendaEvents { get => agendaEvents; set => agendaEvents = value; }
        [XmlArray("SocialNets")]
        [XmlArrayItem("Net")]
        public List<LinkedSocialNetwork> SocialNets { get => socialNets; set => socialNets = value; }

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
                                    string email,
                                    List<ClientLogic.ClientStatus> contacts,
                                    List<Message> messages,
                                    List<AgendaEvent> agendaEvents,
                                    List<LinkedSocialNetwork> socialNets
                                    ) : base(type, alea)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            PhoneNumber = phoneNumber;
            Gender = genre;
            Username = username;
            Password = password;
            Contacts = contacts;
            Email = email;
            Messages = messages;
            AgendaEvents = agendaEvents;
            SocialNets = socialNets;
        }

        public override string ToString()
        {
            return base.ToString() + String.Format(", FirstName={0}, LastName={1}, Age={2}, PhoneNumber={3}" + 
                                       ", Gender={4}, Email={5}, Username={6}, Password={7}]",
                                       FirstName, LastName, Age, PhoneNumber, Gender.ToString(), Email,
                                       Username, Password);
        }
    }
}
