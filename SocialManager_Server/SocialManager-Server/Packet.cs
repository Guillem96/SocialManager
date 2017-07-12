using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketTypes;

namespace SocialManager_Server
{
    class Packet
    {
        private PacketTypes.PacketTypes type;
        private string username;
        private int alea;
        private byte[] data;

        public PacketTypes.PacketTypes Type { get => type; set => type = value; }
        public string Username { get => username; set => username = value; }
        public int Alea { get => alea; set => alea = value; }
        public byte[] Data { get => data; set => data = value; }
    }
}
