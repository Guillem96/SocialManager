using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Timers;
using SocialManager_Client.Connections;

namespace SocialManager_Client
{
    public partial class Client
    {
        private Connections.UDPConnection udp;
        private Connections.TCPConnection tcp;

        private Profile profile;
        private string alea;
        private Timer aliveTimer;

        internal UDPConnection Udp { get => udp; set => udp = value; }
        internal TCPConnection Tcp { get => tcp; set => tcp = value; }
        public Profile Profile { get => profile; set => profile = value; }
        public string Alea { get => alea; set => alea = value; }
        public Timer AliveTimer { get => aliveTimer; set => aliveTimer = value; }

        public Client()
        {
            // TODO: Read server info from a file
            Udp = new Connections.UDPConnection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000));
            Alea = "0000000";
            Profile = new Profile();

            // Set timer to send alives every 5 seconds.
            // Timer will be enabled afer logging
            // Timer will execute alive inf packet sender
            // Timer will execute profile updater for keep contacts status
            AliveTimer = new Timer();
            AliveTimer.Interval = 5000;
            AliveTimer.Elapsed += (sender, e) => KeepAlive();
            string aux = "";
            AliveTimer.Elapsed += (sender, e) => UpdateProfile(Profile, out aux);

            AliveTimer.Enabled = false;
        }

        public bool Register(Profile newProfile, out string message)
        {
            return UdpUtilities.Account.Register(newProfile, this, out message);
        }

        public bool Login(string username, string password, out string message)
        {
            return UdpUtilities.Account.Login(username, password, this, out message);
        }

        public bool Logout(out string message)
        {
            return UdpUtilities.Account.Logout(this, out message);
        }

        public bool DeleteAccount(out string message)
        {
            return UdpUtilities.Account.DeleteAccount(this, out message);
        }

        public bool UpdateProfile(Profile newProfile, out string message)
        {
            return UdpUtilities.Alive.UpdateProfile(newProfile, this, out message);  
        }

        public void KeepAlive()
        {
            UdpUtilities.Alive.UpdateProfile(this);
        }

        public bool ClientsQuery(string query, out string message, ref List<Profile> profiles)
        {
            return UdpUtilities.Contact.ClientQuery(query, ref profiles, this, out message);
        }

        public bool GetContactRequestList(out string message)
        {
            return UdpUtilities.Contact.GetContactRequests(this, out message);
        }

        public bool SendContactRequest(string to, out string message)
        {
            return UdpUtilities.Contact.SendContactRequest(to, this, out message);
        }

        public bool AnswerContactRequest(ContactRequest req, bool ack, out string message)
        {
            return UdpUtilities.Contact.AnswerContactRequest(req, ack, this, out message);
        }

        public void DebugInfo(string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + message);
        }
    }
    
}
