﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Client.Packets
{
    [Serializable]
    [XmlRoot(ElementName = "Packet")]
    public class AlivePacket : Packet
    {
        private string username;

        [XmlElement(ElementName = "Username")]
        public string Username { get => username; set => username = value; }

        public AlivePacket() : base() { }

        internal AlivePacket(PacketTypes type, string alea, string username) : base(type, alea)
        {
            Username = username;
        }

        public override string ToString()
        {
            return base.ToString() + ", Username=" + Username + "]";
        }
    }
}
