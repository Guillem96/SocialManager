using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client.UdpUtilities
{
    static class Alive
    {
        public static bool UpdateProfile(Profile newProfile, Client client, out string message)
        {
            try
            {
                // Pack login request packet
                Packets.ProfilePacket profilePacket = new Packets.ProfilePacket(
                                                    Packets.PacketTypes.ProfileUpdateReq,
                                                    client.Alea,
                                                    newProfile
                                                );

                // Send login request package
                client.Udp.SendMessage(profilePacket.Pack());

                // Recieve the data
                var data = client.Udp.RecieveMessage();
                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.ProfileUpdateAck:
                        // Complete profile update
                        Packets.ProfilePacket profileP =
                            Packets.Packet.Unpack<Packets.ProfilePacket>(data);
                        
                        // No set new messages when alive
                        profileP.Messages = client.Profile.Messages;

                        client.Profile.SetFromPacket(profileP);
                        message = "Profile Updated";
                        return true;
                    case Packets.PacketTypes.Error:
                        message = "Error: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        client.DebugInfo("Update Profile: " + message);
                        break;
                    default:
                        client.DebugInfo("Update Profile: Unexpected type.");
                        message = "Error, unexpected type.";
                        break;
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException e)
            {
                client.DebugInfo("Server is offline.");
                message = e.ToString();
                return false;
            }
        }

        public static void UpdateProfile(Client client)
        {
            try
            {
                // Pack alive info packet
                byte[] alive = new Packets.BasicReqPacket(
                                                    Packets.PacketTypes.AliveInf,
                                                    client.Alea,
                                                    client.Profile.Username
                                                    ).Pack();

                // Send alive packet
                client.Udp.SendMessage(alive);

                // Recieve the data
                var data = client.Udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.AliveAck:
                        // ALIVE CORRECT
                        client.DebugInfo("Alive: Alive ack recieved");
                        break;
                    case Packets.PacketTypes.Error:
                        client.DebugInfo("Alive: " + Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message);
                        client.DebugInfo("Login: Alive timer is disabled.");
                        client.AliveTimer.Enabled = false;
                        return;
                    default:
                        client.DebugInfo("Alive unexpected alive type");
                        client.DebugInfo("Login: Alive timer is disabled.");
                        client.AliveTimer.Enabled = false;
                        return;
                }

            }
            catch (System.Net.Sockets.SocketException)
            {
                client.DebugInfo("Server is offline.");
                return;
            }
        }
    }
}
