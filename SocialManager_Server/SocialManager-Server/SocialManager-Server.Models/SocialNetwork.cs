using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Server.Models
{
    [Table(Name="SocialNetworks")]
    public class SocialNetwork
    {
        private int socialNetworkID;
        private string name;

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int SocialNetworkID { get => socialNetworkID; set => socialNetworkID = value; }

        [Column(CanBeNull = false)]
        public string Name { get => name; set => name = value; }

        public SocialNetwork() { }
    }
}
