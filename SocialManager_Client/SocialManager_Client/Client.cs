using System;
using System.Net;


namespace SocialManager_Client
{
    class Client
    {
        private Connections.UDPConnection udp;
        private Profile profile;
        private string alea;

        public Client()
        {
            // TODO: Read server info from a file
            udp = new Connections.UDPConnection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000));
            string alea = "0000000";
            profile = new Profile();
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
                        message = "HE SIDO REGISTRADOOOO!!!";
                        return true;
                    case Packets.PacketTypes.Error:
                        message= "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        break;
                    default:
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch(System.Net.Sockets.SocketException)
            {
                message = "Server is offline.";
                return false;
            }
        }

        public bool Login(string username, string password, out string message)
        {
            try
            {
                // Pack register request packet
                Packets.LoginReqPacket log = new Packets.LoginReqPacket(
                                                    Packets.PacketTypes.LoginReq, 
                                                    "0000000",
                                                    username, 
                                                    password
                                                );

                // Send register request package
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
                        SetProfileFromPacket(profileP);
                        message = "I'm logged in.";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        break;
                    default:
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                message = "Server is offline.";
                return false;
            }
        }

        private void SetProfileFromPacket(Packets.ProfilePacket p)
        {
            profile.FirstName = p.FirstName;
            profile.LastName = p.LastName;
            profile.PhoneNumber = p.PhoneNumber;
            profile.Age = p.Age;
            profile.Username = p.Username;
            profile.Password = p.Password;
            profile.Email = p.Email;
            profile.Genre = p.Genre;
            alea = p.Alea;
        }
    }
}
