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
            aliveTimer = new Timer();
            aliveTimer.Interval = 5000;
            aliveTimer.Elapsed += (sender, e) => KeepAlive();
            aliveTimer.Enabled = false;
        }

        internal bool Register(Profile newProfile, out string message)
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
                        message= "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        DebugInfo("Register: " + message);
                        break;
                    default:
                        DebugInfo("Register: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch(System.Net.Sockets.SocketException)
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

        public void KeepAlive()
        {
            try
            {
                // Pack alive info packet
                byte[] alive = new Packets.AlivePacket(
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
