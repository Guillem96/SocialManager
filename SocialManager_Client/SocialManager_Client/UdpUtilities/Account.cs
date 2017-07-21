using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client.UdpUtilities
{
    static class Account
    {
        public static bool Register(Profile newProfile, Client client, out string message)
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
                                (Profile.Sex)newProfile.Gender,
                                newProfile.Username,
                                newProfile.Password,
                                newProfile.Email
                                );

                // Send register request package
                client.Udp.SendMessage(reg.Pack());

                // Recieve the data
                var data = client.Udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.RegisterAck:
                        // Complete registration
                        client.DebugInfo("Register: Done!");
                        message = "Registration successfully completed. Now You can login.";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        client.DebugInfo("Register: " + message);
                        break;
                    default:
                        client.DebugInfo("Register: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                client.DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }

        public static bool DeleteAccount(Client client, out string message)
        {
            try
            {
                // Pack logut request packet
                // use alive packet because contains the same information as delete account request
                Packets.BasicReqPacket delete = new Packets.BasicReqPacket(
                                                    Packets.PacketTypes.DeleteAccountReq,
                                                    client.Alea,
                                                    client.Profile.Username
                                                );

                // Send delete account request package
                client.Udp.SendMessage(delete.Pack());

                // Recieve the data
                var data = client.Udp.RecieveMessage();
                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.DeleteAccountAck:
                        // Complete Deletion
                        client.DebugInfo("Account delete: Done.");
                        message = "";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        client.DebugInfo("Delete account error: " + message);
                        break;
                    default:
                        client.DebugInfo("Delete account: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                client.DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }

        public static bool Logout(Client client, out string message)
        {
            try
            {
                // Pack logut request packet
                // use alive packet because contains the same information as logut request
                Packets.BasicReqPacket logout = new Packets.BasicReqPacket(
                                                    Packets.PacketTypes.LogoutReq,
                                                    client.Alea,
                                                    client.Profile.Username
                                                );

                // Send logut request package
                client.Udp.SendMessage(logout.Pack());

                // Recieve the data
                var data = client.Udp.RecieveMessage();
                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.LogoutAck:
                        // Complete logut
                        client.DebugInfo("Logut: Done.");
                        message = "I'm NOT logged in now.";
                        client.AliveTimer.Enabled = false;
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        client.DebugInfo("Logut: " + message);
                        break;
                    default:
                        client.DebugInfo("Logout: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                client.DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }

        public static bool Login(string username, string password, Client client, out string message)
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
                client.Udp.SendMessage(log.Pack());

                // Recieve the data
                var data = client.Udp.RecieveMessage();
                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.LoginAck:
                        // Complete login
                        Packets.ProfilePacket profileP = Packets.Packet.Unpack<Packets.ProfilePacket>(data);
                        client.Profile.SetFromPacket(profileP);
                        Console.WriteLine(Encoding.ASCII.GetString(data));
                        client.Alea = p.Alea;
                        client.DebugInfo("Login: Done.");
                        client.DebugInfo("Login: Alive timer is enabled.");
                        client.AliveTimer.Enabled = true;
                        message = "I'm logged in.";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        client.DebugInfo("Login: " + message);
                        break;
                    default:
                        client.DebugInfo("Login: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                client.DebugInfo("Server is offline.");
                message = "Server is offline.";
                return false;
            }
        }
    }
}
