using System;
using System.Net;


namespace SocialManager_Client
{
    class Client
    {
        private Connections.UDPConnection udp;

        public Client()
        {
            // TODO: Read server info from a file
            udp = new Connections.UDPConnection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000));
        }

        public bool Register(Profile newProfile, out string message)
        {
            try
            {
                // Pack register request packet
                Packets.RegisterReqPacket reg =
                            new Packets.RegisterReqPacket(
                                "0000000", //> For registering always 7 0's
                                newProfile.FirstName,
                                newProfile.LastName,
                                newProfile.Age,
                                newProfile.PhoneNumber,
                                newProfile.Genre,
                                newProfile.Username,
                                newProfile.PhoneNumber,
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
    }
}
