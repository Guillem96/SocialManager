﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SocialManager_Server.Packets;
using SocialManager_Server.ClientLogic;

using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.ServerLogic
{
    static class UdpUtilities
    {
        public static void Register(byte[] data, IPEndPoint ip, Server server)
        {
            Models.Client current = null;   //< Who is doing the request
            string message = "";            

            // Unpack the packet in the correct format
            ProfilePacket regPacket = Packet.Unpack<ProfilePacket>(data);

            server.DebugInfo("Register request recieved.");
            server.DebugInfo("RegisterReq Packet: " + regPacket.ToString());
            // Send a package depending on registration success
            if (ClientsManagement.RegisterClient(regPacket, ip, ref current, out message))
            {
                // Send RegisterAck if all data is correct
                server.Udp.SendMessage(new AckErrorPacket(
                                        PacketTypes.RegisterAck,
                                        "Congratulations now you are registered.").Pack(),
                                        ip);

                server.DebugInfo(regPacket.FirstName + " " + regPacket.LastName + " has been registered as " + regPacket.Username);
                server.Clients.Add(new ClientStatus(current));
            }
            else
            {
                // Send the error
                server.DebugInfo("Register: " + regPacket.FirstName + " " + regPacket.LastName + " request not accepted");
                server.Udp.SendError(message, ip);
            }
        }

        public static void Login(byte[] data, IPEndPoint ip, Server server)
        {
            Models.Client current = null;   //< Who is doing the request
            string message = "";
            
            // Unpack
            LoginReqPacket logPacket = Packet.Unpack<LoginReqPacket>(data);

            server.DebugInfo("Login request recieved.");
            server.DebugInfo("LoginReq Packet: " + logPacket.ToString());

            if (ClientsManagement.LoginClient(logPacket, ref current, out message))
            {
                // Client login 
                server.DebugInfo("Client " + current.ToString() + " is now logged in.");
                string alea = Server.GenerateAlea();
                server.ChangeStatus(logPacket.Username, ClientStatus.Status.Logged, alea, DateTime.Now);
                // Send the profile info of the database to client
                server.DebugInfo("Sending profile info to " + logPacket.Username + ".");
                server.Udp.SendMessage(new Packets.ProfilePacket(
                                            Packets.PacketTypes.LoginAck,
                                            alea, // New alea generated
                                            current.FirstName,
                                            current.LastName,
                                            current.Age,
                                            current.PhoneNumber,
                                            current.Genre,
                                            current.Username,
                                            current.Password).Pack(), ip);
            }
            else
            {
                server.DebugInfo("Login: " + logPacket.Username + " request not accepted");
                server.Udp.SendError(message, ip);
            }
        }

        public static void Alive(byte[] data, IPEndPoint ip, Server server)
        {
            string message = "";

            AlivePacket aPacket = Packet.Unpack<AlivePacket>(data);

            server.DebugInfo("Alive infrecieved.");
            server.DebugInfo("AliveInf Packet: " + aPacket.ToString());

            ClientStatus current = server.GetClient(aPacket.Username);

            if(ClientsManagement.AliveClient(aPacket, current, out message))
            {
                // Save the last alive
                current.LastAlive = DateTime.Now;
                // Send ack
                server.Udp.SendMessage(new AlivePacket(PacketTypes.AliveAck, aPacket.Alea, aPacket.Username).Pack(), ip);
            }
            else
            {
                server.DebugInfo("Alive: Incorrect alea number from " + aPacket.Username);
                server.DebugInfo(aPacket.Username + " now is disconnected.");
                // Disconnect the client
                current.Disconnect();
                // Send error
                server.Udp.SendError(message, ip);
            }
            
        }

        public static void CheckAlives(Server server)
        {
            foreach(ClientStatus cs in server.Clients.Where(c=>c.Stat != ClientStatus.Status.Disconnected))
            {  
                TimeSpan since = DateTime.Now - cs.LastAlive;
                if(since.Seconds > 12)
                {
                    cs.Disconnect();
                    server.DebugInfo(cs.Client.Username + " now is disconnected because he is inactive.");
                }    
            }
        }
    }
}