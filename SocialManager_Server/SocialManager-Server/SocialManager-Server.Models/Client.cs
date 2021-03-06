﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Serialization;
using System.Data.Linq;

namespace SocialManager_Server.Models
{
    /// <summary>
    /// Database models. Stores client information.
    /// </summary>
    [Table(Name ="Clients")]
    [Serializable]
    [XmlRoot("Client")]
    public class Client
    {
        public enum Sex { Male, Female }

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
        [XmlIgnore]
        public int ClientID { get => clientID; set => clientID = value; }

        [Column(CanBeNull = false)]
        [XmlElement]
        public string FirstName { get => firstName; set => firstName = value; }

        [Column(CanBeNull = false)]
        [XmlElement]
        public string LastName { get => lastName; set => lastName = value; }

        [Column(CanBeNull = false)]
        public string Username { get => username; set => username = value; }

        [Column(CanBeNull = false)]
        [XmlElement]
        public string Password { get => password; set => password = value; }

        [Column]
        [XmlElement]
        public int Age { get => age; set => age = value; }

        [Column]
        [XmlElement]
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }

        [Column(CanBeNull = false)]
        [XmlElement]
        public Sex Gender { get => genre; set => genre = value; }

        [Column]
        [XmlElement]
        public string Email { get => email; set => email = value; }

        [Column(CanBeNull = false)]
        [XmlIgnore]
        public byte[] Ip { get => ip; set => ip = value; }


        public override string ToString()
        {
            return Username + " - " + FirstName + " " + LastName;
        }
    }
}
