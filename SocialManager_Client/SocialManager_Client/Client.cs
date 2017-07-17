using System;
using System.Net;
using System.Text;
using System.Timers;

namespace SocialManager_Client
{
    public class Client
    {
        private Connections.UDPConnection udp;
        private Profile profile;
        private string alea;
        private Timer aliveTimer;
        
        public Profile Profile { get => profile; set => profile = value; }

        public Client()
        {
            // TODO: Read server info from a file
            udp = new Connections.UDPConnection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000));
            alea = "0000000";
            Profile = new Profile();

            // Set timer to send alives every 5 seconds.
            // Timer will be enabled afer logging
            // Timer will execute alive inf packet sender
            // Timer will execute profile updater for keep contacts status
            aliveTimer = new Timer();
            aliveTimer.Interval = 5000;
            aliveTimer.Elapsed += (sender, e) => KeepAlive();
            string aux = "";
            aliveTimer.Elapsed += (sender, e) => UpdateProfile(profile, out aux);

            aliveTimer.Enabled = false;
        }

        public bool Register(Profile newProfile, out string message)
        {
            try
            {
                // Pack register request packet
                Packets.ProfilePacket reg =
                            new Packets.ProfilePacket(
                                Packets.PacketTypes.RegisterReq,
                                "0000000", //> For registering always 7 0's
                                newProfile.FirstName,
                                newProfile.LastName,
                                newProfile.Age,
                                newProfile.PhoneNumber,
                                newProfile.Genre,
                                newProfile.Username,
                                newProfile.Password,
                                newProfile.Email
                                );

                // Send register request package
                udp.SendMessage(reg.Pack());

                // Recieve the data
                var data = udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.RegisterAck:
                        // Complete registration
                        DebugInfo("Register: Done!");
                        message = "HE SIDO REGISTRADOOOO!!!";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        DebugInfo("Register: " + message);
                        break;
                    default:
                        DebugInfo("Register: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }

        public bool Login(string username, string password, out string message)
        {
            try
            {
                // Pack login request packet
                Packets.LoginReqPacket log = new Packets.LoginReqPacket(
                                                    Packets.PacketTypes.LoginReq,
                                                    "0000000",
                                                    username,
                                                    password
                                                );

                // Send login request package
                udp.SendMessage(log.Pack());

                // Recieve the data
                var data = udp.RecieveMessage();
                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.LoginAck:
                        // Complete login
                        Packets.ProfilePacket profileP = Packets.Packet.Unpack<Packets.ProfilePacket>(data);
                        Profile.SetFromPacket(profileP);
                        alea = p.Alea;
                        DebugInfo("Login: Done.");
                        DebugInfo("Login: Alive timer is enabled.");
                        aliveTimer.Enabled = true;
                        message = "I'm logged in.";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        DebugInfo("Login: " + message);
                        break;
                    default:
                        DebugInfo("Login: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }

        public bool Logout(out string message)
        {
            try
            {
                // Pack logut request packet
                // use alive packet because contains the same information as logut request
                Packets.BasicReqPacket logout = new Packets.BasicReqPacket(
                                                    Packets.PacketTypes.LogoutReq,
                                                    alea,
                                                    profile.Username
                                                );

                // Send logut request package
                udp.SendMessage(logout.Pack());

                // Recieve the data
                var data = udp.RecieveMessage();
                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.LogoutAck:
                        // Complete logut
                        DebugInfo("Logut: Done.");
                        message = "I'm NOT logged in now.";
                        aliveTimer.Enabled = false;
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        DebugInfo("Logut: " + message);
                        break;
                    default:
                        DebugInfo("Logout: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }

        public bool UpdateProfile(Profile newProfile, out string message)
        {
            try
            {
                // Pack login request packet
                Packets.ProfilePacket profilePacket = new Packets.ProfilePacket(
                                                    Packets.PacketTypes.ProfileUpdateReq,
                                                    alea,
                                                    newProfile
                                                );

                // Send login request package
                udp.SendMessage(profilePacket.Pack());

                // Recieve the data
                var data = udp.RecieveMessage();
                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.ProfileUpdateAck:
                        // Complete login
                        Packets.ProfilePacket profileP = Packets.Packet.Unpack<Packets.ProfilePacket>(data);
                        Profile.SetFromPacket(profileP);
                        message = "Profile Updated";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        DebugInfo("Update Profile: " + message);
                        break;
                    default:
                        DebugInfo("Update Profile: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }

        public bool DeleteAccount(out string message)
        {
            try
            {
                // Pack logut request packet
                // use alive packet because contains the same information as delete account request
                Packets.BasicReqPacket delete = new Packets.BasicReqPacket(
                                                    Packets.PacketTypes.DeleteAccountReq,
                                                    alea,
                                                    profile.Username
                                                );

                // Send delete account request package
                udp.SendMessage(delete.Pack());

                // Recieve the data
                var data = udp.RecieveMessage();
                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.DeleteAccountAck:
                        // Complete Deletion
                        DebugInfo("Account delete: Done.");
                        message = "";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        DebugInfo("Delete account error: " + message);
                        break;
                    default:
                        DebugInfo("Delete account: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }

        }

        public bool GetContactRequestList(out string message)
        {
            try
            {
                // Prepare the packet to send
                Packets.BasicReqPacket packet = new Packets.BasicReqPacket(
                                                        Packets.PacketTypes.ListContactReq,
                                                        alea,
                                                        profile.Username
                                                    );

                // send the package
                udp.SendMessage(packet.Pack());

                // Recieve packet
                byte[] data = udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.ContactAck:
                        // Update contact requests list
                        Packets.ContactReqListPacket contactsRequests = 
                                                    Packets.Packet.Unpack<Packets.ContactReqListPacket>(data);

                        profile.RecievedContactRequests = contactsRequests.Recieved;
                        profile.SentContactRequests = contactsRequests.Sent;

                        message = "Recieved list correctly";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        DebugInfo("Contact request list: " + message);
                        break;
                    default:
                        DebugInfo("Contact request list: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }

        public bool SentContactRequest(string to, out string message)
        {
            try
            {
                // Prepare the packet to send
                Packets.ContactReqPacket packet = new Packets.ContactReqPacket(
                                                        Packets.PacketTypes.NewContactReq,
                                                        alea,
                                                        profile.Username,
                                                        to
                                                    );

                // send the package
                udp.SendMessage(packet.Pack());

                // Recieve packet
                byte[] data = udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.ContactAck:
                        DebugInfo("Send Contact request: Sent succesful to " + to);

                        // Update the request list
                        GetContactRequestList(out message);

                        message = "Request sent correctly.";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        DebugInfo("Send Contact request: " + message);
                        break;
                    default:
                        DebugInfo("Send Contact request: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }

        public bool AnswerContactRequest(ContactRequest req, bool ack, out string message)
        {
            try
            {
                // Prepare the packet to send
                Packets.ContactReqPacket packet = new Packets.ContactReqPacket(
                                                        (ack) ? Packets.PacketTypes.AcceptNewContact : Packets.PacketTypes.RegNewContact,
                                                        alea,
                                                        req.From.Username,
                                                        profile.Username
                                                    );

                // send the package
                udp.SendMessage(packet.Pack());

                // Recieve packet
                byte[] data = udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.ContactAck:
                        if (ack)
                        {
                            // Todo: Get Contacts info
                            DebugInfo("Answer Contact Request: Accepted " + req.From.Username + " correctly.");
                        }
                        else
                        {
                            DebugInfo("Answer Contact Request: Refused " + req.From.Username + " correctly.");
                        }

                        // Update the request list
                        GetContactRequestList(out message);

                        message = "Answered correctly";
                        return true;

                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        DebugInfo("Answer Contact Request: " + message);
                        break;
                    default:
                        DebugInfo("Answer Contact Request: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }

        public void KeepAlive()
        {
            try
            {
                // Pack alive info packet
                byte[] alive = new Packets.BasicReqPacket(
                                                    Packets.PacketTypes.AliveInf,
                                                    alea,
                                                    Profile.Username
                                                    ).Pack();

                // Send alive packet
                udp.SendMessage(alive);

                // Recieve the data
                var data = udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.AliveAck:
                        // ALIVE CORRECT
                        DebugInfo("Alive: Alive ack recieved");
                        break;
                    case Packets.PacketTypes.Error:
                        DebugInfo("Alive: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message);
                        DebugInfo("Login: Alive timer is disabled.");
                        aliveTimer.Enabled = false;
                        return;
                    default:
                        DebugInfo("Alive unexpected alive type");
                        DebugInfo("Login: Alive timer is disabled.");
                        aliveTimer.Enabled = false;
                        return;
                }

            }
            catch (System.Net.Sockets.SocketException)
            {
                DebugInfo("Server is offline.");
                return;
            }
        }

        public void DebugInfo(string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + message);
        }
    }
    
}
