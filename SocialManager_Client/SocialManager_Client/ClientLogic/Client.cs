using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Timers;
using SocialManager_Client.Connections;
using System.Windows;
using System.Threading;
using System.Net.Sockets;
using SocialManager_Client.SocialNetworksLogic;

namespace SocialManager_Client
{
    public partial class Client
    {
        private UDPConnection udp;
        private TCPConnection tcp;

        private Profile profile;
        private string alea;
        private System.Timers.Timer aliveTimer;
        private bool chatAlive;
        private Thread tcpTask;
        private Packets.MessagePacket buffer;

        private SocialNetworksLogic.Twitter twitter = null;
        internal Twitter Twitter { get => twitter; set => twitter = value; }

        internal UDPConnection Udp { get => udp; set => udp = value; }
        internal TCPConnection Tcp { get => tcp; set => tcp = value; }
        public Profile Profile { get => profile; set => profile = value; }
        public string Alea { get => alea; set => alea = value; }
        public System.Timers.Timer AliveTimer { get => aliveTimer; set => aliveTimer = value; }
        public bool ChatAlive { get => chatAlive; set => chatAlive = value; }

        public Client()
        {
            // TODO: Read server info from a file
            Udp = new UDPConnection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000));

            ChatAlive = false;

            Alea = "0000000";
            Profile = new Profile();

            // Set timer to send alives every 5 seconds.
            // Timer will be enabled afer logging
            // Timer will execute alive inf packet sender
            // Timer will execute profile updater for keep contacts status
            AliveTimer = new System.Timers.Timer();
            AliveTimer.Interval = 5000;
            AliveTimer.Elapsed += (sender, e) => KeepAlive();
            string aux = "";
            AliveTimer.Elapsed += (sender, e) => UpdateProfile(Profile, out aux);

            AliveTimer.Enabled = false;
        }

        public bool Register(Profile newProfile, out string message)
        {
            return UdpUtilities.Register(newProfile, this, out message);
        }

        public bool Login(string username, string password, out string message)
        {
            bool b = UdpUtilities.Login(username, password, this, out message);
            
            // Start tcp after login
            if (b)
            {
                DebugInfo("Tcp chat started.");
                tcpTask = new Thread(() => TcpChat());
                tcpTask.Start();
            }
            
            return b;
        }

        public bool Logout(out string message)
        {
            tcpTask.Abort();
            return UdpUtilities.Logout(this, out message);
        }

        public bool DeleteAccount(out string message)
        {
            return UdpUtilities.DeleteAccount(this, out message);
        }

        public bool UpdateProfile(Profile newProfile, out string message)
        {
            return UdpUtilities.UpdateProfile(newProfile, this, out message);  
        }

        public void KeepAlive()
        {
            UdpUtilities.UpdateProfile(this);
        }

        public bool ClientsQuery(string query, out string message, ref List<Profile> profiles)
        {
            return UdpUtilities.ClientQuery(query, ref profiles, this, out message);
        }

        public bool GetContactRequestList(out string message)
        {
            return UdpUtilities.GetContactRequests(this, out message);
        }

        public bool SendContactRequest(string to, out string message)
        {
            return UdpUtilities.SendContactRequest(to, this, out message);
        }

        public bool AnswerContactRequest(ContactRequest req, bool ack, out string message)
        {
            return UdpUtilities.AnswerContactRequest(req, ack, this, out message);
        }

        public void TcpChat()
        {
            try
            {
                Tcp = new TCPConnection();

                if (ChatAliveRequest())
                {
                    DebugInfo("Chat function has started.");
                    // Listening messages
                    while (true)
                    {
                        byte[] data = Tcp.RecieveMessage();

                        // Unpack
                        Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);

                        switch ((Packets.PacketTypes)p.Type)
                        {
                            case Packets.PacketTypes.SendMessageAck:
                                Packets.MessagePacket mPacket = 
                                    Packets.Packet.Unpack<Packets.MessagePacket>(data);

                                DebugInfo("MessageRecieved: " + mPacket.ToString());

                                // Add message to profile
                                Profile.Messages.Add(new Message()
                                {
                                    To = mPacket.To,
                                    From = mPacket.From,
                                    Content = mPacket.Content,
                                    Read = true,
                                    DateString = mPacket.DateString
                                });
                                break;
                            case Packets.PacketTypes.Error:
                                // Error sending the message at the buffer
                                DebugInfo("Error Sending: " + buffer.ToString());
                                MessageBox.Show("Error sending message.");
                                break;
                        }
                        
                    }
                }
            }
            catch(ThreadAbortException)
            {
                return;
            }
            catch (SocketException e)
            {
                ChatAlive = false;
                MessageBox.Show("Imposible chatear. Server offline.");
                DebugInfo(e.ToString());
                return;
            }
            catch (System.IO.IOException e )
            {
                ChatAlive = false;
                MessageBox.Show("Imposible chatear. Server offline.");
                DebugInfo(e.ToString());
                return;
            }
        }

        public bool SendChatMessage(string destUser, string content, out string message)
        {
            if (!ChatAlive)
            {
                message = "Chat is not available.";
                return false;
            }
            // Check if destUser is your contact
            if(!Profile.Contacts.Exists (c => c.Profile.Username == destUser))
            {
                message = destUser + " is not your friend.";
                return false;
            }

            message = "";
            buffer = new Packets.MessagePacket(Packets.PacketTypes.SendMessageReq,
                                                            alea,
                                                            Profile,
                                                            Profile.Contacts.Find(c => c.Profile.Username == destUser).Profile,
                                                            content,
                                                            false);

            Tcp = new TCPConnection();
            Tcp.SendMessage(buffer.Pack());

            DebugInfo("Message sended: " + buffer.ToString());
            return true;
        }

        private bool ChatAliveRequest()
        {
            // First request to be alive on chat
            Tcp.SendMessage(new Packets.BasicReqPacket(
                                        Packets.PacketTypes.ReadyChatReq,
                                        alea,
                                        Profile.Username).Pack());

            // Recieve the packet
            byte[] data = Tcp.RecieveMessage();

            // Unpack the data and check if all is correct
            Packets.AckErrorPacket p = Packets.Packet.Unpack<Packets.AckErrorPacket>(data);

            switch ((Packets.PacketTypes)p.Type)
            {
                case Packets.PacketTypes.Error:
                    MessageBox.Show("Chat is not available.\n" + p.Message);
                    return false;
                case Packets.PacketTypes.ReadyChatAck:
                    ChatAlive = true;
                    return true;
                default:
                    MessageBox.Show("Unexpected packet.");
                    return false;
            }
        }

        public bool LinkNewSocialNetwork(SocialNetwork sn, out string message)
        {
            return UdpUtilities.LinkSocialNetwork(sn, this, out message);
        }

        public bool DeleteLinkNewSocialNetwork(SocialNetwork sn, out string message)
        {
            return UdpUtilities.DeleteLinkSocialNetwork(sn, this, out message);
        }

        public bool AddNewAgendaEvent(AgendaEvent e, out string message)
        {
            return UdpUtilities.AgendaEvent(e.EventName, e.EventInfo, e.Date, false, this, out message);
        }

        public bool DeleteAgendaEvent(AgendaEvent e, out string message)
        {
            return UdpUtilities.AgendaEvent(e.EventName, e.EventInfo, e.Date, true, this, out message);
        }

        internal void TwitterLogin()
        {
            if (Twitter != null) return;

            if (profile.SocialNets.Any(c => c.Name == "Twitter"))
            {
                SocialNetwork t = profile.SocialNets.Single(c => c.Name == "Twitter");
                Twitter = new SocialNetworksLogic.Twitter(t.Username, t.Password);
            }
            else
                Twitter = null;
        }

        public void DebugInfo(string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + message);
        }
    }
    
}
