using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Client
{
    /// <summary>
    /// Client profile
    /// </summary>
    [Serializable]
    [XmlRoot("Profile")]
    public class Profile
    {
        public enum Sex { Male, Female }

        private string firstName;
        private string lastName;
        private int age;
        private string phoneNumber;
        private Sex gender;
        private string username;
        private string password;
        private string email;
        private List<Contact> contacts;
        private List<ContactRequest> recieved;
        private List<ContactRequest> sent;
        private List<Message> messages;
        private List<AgendaEvent> agendaEvents;
        private List<SocialNetwork> socialNets;

        [XmlElement("FirstName")]
        public string FirstName { get => firstName; set => firstName = value; }
        [XmlElement("LastName")]
        public string LastName { get => lastName; set => lastName = value; }
        [XmlElement("Age")]
        public int Age { get => age; set => age = value; }
        [XmlElement("PhoneNumber")]
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        [XmlElement("Gender")]
        public Sex Gender { get => gender; set => gender = value; }
        [XmlElement("Username")]
        public string Username { get => username; set => username = value; }
        [XmlElement("Password")]
        public string Password { get => password; set => password = value; }
        [XmlElement("Email")]
        public string Email { get => email; set => email = value; }
        [XmlIgnore]
        public List<Contact> Contacts { get => contacts; set => contacts = value; }
        [XmlIgnore]
        public List<ContactRequest> RecievedContactRequests { get => recieved; set => recieved = value; }
        [XmlIgnore]
        public List<ContactRequest> SentContactRequests { get => sent; set => sent = value; }
        [XmlIgnore]
        public List<Message> Messages { get => messages; set => messages = value; }
        [XmlIgnore]
        public List<AgendaEvent> AgendaEvents { get => agendaEvents; set => agendaEvents = value; }
        [XmlIgnore]
        public List<SocialNetwork> SocialNets { get => socialNets; set => socialNets = value; }


        public Profile() { }

        internal Profile(string firstName, 
                        string lastName, 
                        int age, 
                        string phoneNumber, 
                        Sex genre, 
                        string username, 
                        string password,
                        string email,
                        List<Contact> contacts,
                        List<Message> messages,
                        List<AgendaEvent> events,
                        List<SocialNetwork> socialNets
                        )
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            PhoneNumber = phoneNumber;
            Gender = genre;
            Username = username;
            Password = password;
            Email = email;
            Contacts = contacts;
            recieved = new List<ContactRequest>();
            sent = new List<ContactRequest>();
            Messages = messages;
            AgendaEvents = events;
            SocialNets = socialNets;
        }

        public void SetFromPacket(Packets.ProfilePacket p)
        {     
            FirstName = p.FirstName;
            LastName = p.LastName;
            PhoneNumber = p.PhoneNumber;
            Age = p.Age;
            Username = p.Username;
            Password = p.Password;
            Email = p.Email;
            Gender = p.Gender;
            Contacts = p.Contacts;
            Messages = p.Messages;
            AgendaEvents = p.AgendaEvents;
            SocialNets = p.SocialNets;
        }

        public override string ToString()
        {
            return "First Name: " + FirstName + Environment.NewLine +
                    "Last Name: " + LastName + Environment.NewLine +
                    "Age: " + Age + Environment.NewLine +
                    //"Phone Number: " + PhoneNumber + Environment.NewLine +
                    "Username: " + Username + Environment.NewLine +
                    //"Email: " + Email + Environment.NewLine +
                    "Gender: " + Gender.ToString() + Environment.NewLine +
                    "Contacts: " + String.Join(Environment.NewLine + "\t", Contacts.Select(c=> c.Profile.Username + " - " + c.Stat.ToString()));
        }
    }
}
