using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client
{
	static partial class UdpUtilities
	{
		public static bool LinkSocialNetwork(SocialNetwork sn, Client client, out string message)
        {
            try
            {
                // Pack register request packet
                Packets.LinkSocialNetworkPacket packet =
                            new Packets.LinkSocialNetworkPacket(
                                Packets.PacketTypes.LinkSocialNetworkReq,
                                client.Alea,
                                client.Profile.Username,
                                sn.Name,
                                sn.Username,
                                sn.Password);


                // Send register request package
                client.Udp.SendMessage(packet.Pack());

                // Recieve the data
                var data = client.Udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.LinkSocialNetworkAck:
                        // New event added correctly
                        client.DebugInfo("Social Network Link: New social net linked.");
                        message = "New social net linked correctly.";
                        return true;

                    case Packets.PacketTypes.Error:
                        message = Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        client.DebugInfo("Social Network Link error: " + message);
                        break;
                    default:
                        client.DebugInfo("Social Network Link error: Unexpected type.");
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

        public static bool DeleteLinkSocialNetwork(SocialNetwork sn, Client client, out string message)
        {

            try
            {
                // Pack register request packet
                Packets.LinkSocialNetworkPacket packet =
                            new Packets.LinkSocialNetworkPacket(
                                Packets.PacketTypes.DeleteLinkSocialNetReq,
                                client.Alea,
                                client.Profile.Username,
                                sn.Name,
                                sn.Username,
                                sn.Password);


                // Send register request package
                client.Udp.SendMessage(packet.Pack());

                // Recieve the data
                var data = client.Udp.RecieveMessage();

                // Unpack the data and check the type
                Packets.Packet p = Packets.Packet.Unpack<Packets.Packet>(data);
                switch ((Packets.PacketTypes)p.Type)
                {
                    case Packets.PacketTypes.DeleteLinkSocialNetAck:
                        // New event added correctly
                        client.DebugInfo("Social Network Link: New social net deleted.");
                        message = "Deleted social net correctly.";
                        return true;

                    case Packets.PacketTypes.Error:
                        message = Packets.Packet.Unpack<Packets.AckErrorPacket>(data).Message;
                        client.DebugInfo("Delete Social Network Link error: " + message);
                        break;
                    default:
                        client.DebugInfo("Social Network Link error: Unexpected type.");
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
