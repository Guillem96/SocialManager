using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SocialManager_Server.Packets;
using SocialManager_Server.ClientLogic;
using System.Text;

namespace SocialManager_Server.ServerLogic
{
    static class UdpUtilitiesAccounts
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
                if (server.GetClient(current.Username).Stat == ClientStatus.Status.Logged)
                {
                    server.DebugInfo("Login: User is already logged");
                    message = "USer is already logged";
                    server.Udp.SendError(message, ip);
                    return;
                }

                // Client login 
                server.DebugInfo("Client " + current.ToString() + " is now logged in.");
                string alea = Server.GenerateAlea();
                server.SetStatus(logPacket.Username, ClientStatus.Status.Logged, alea, DateTime.Now);
                
                // Send the profile info of the database to client
                server.DebugInfo("Sending profile info to " + logPacket.Username + ".");

                // Return user profile with the ack
                server.Udp.SendMessage(new ProfilePacket(
                                            PacketTypes.LoginAck,
                                            alea, // New alea generated
                                            current.FirstName,
                                            current.LastName,
                                            current.Age,
                                            current.PhoneNumber,
                                            current.Gender,
                                            current.Username,
                                            current.Password,
                                            current.Email,
                                            server.GetContacts(server.GetClient(current.Username)),
                                            server.GetUnreadMessages(server.GetClient(current.Username)),
                                            server.GetAgendaEvents(current.Username)
                                        ).Pack(), ip);

                // Mark messages as read
                server.MarkReadMessages(server.GetClient(current.Username));
            }
            else
            {
                server.DebugInfo("Login: Request not accepted");
                server.Udp.SendError(message, ip);
            }
        }

        public static void Logout(byte[] data, IPEndPoint ip, Server server)
        {
            string message = "";

            BasicReqPacket logoutPacket = Packet.Unpack<BasicReqPacket>(data);

            server.DebugInfo("Logut request recieved.");
            server.DebugInfo("LogoutReq Packet: " + logoutPacket.ToString());

            ClientStatus current = server.GetClient(logoutPacket.Username);

            if (ClientsManagement.CheckBasics(current, ClientStatus.Status.Disconnected, logoutPacket.Alea, out message))
            {
                server.DebugInfo("Logout: Correct logut from " + logoutPacket.Username);
                server.DebugInfo(logoutPacket.Username + " now is disconnected.");
                if (server.ClientsOnChat.Any(d => d.Key == current.Client.Username))
                    server.ClientsOnChat.Remove(current.Client.Username);
                current.Disconnect();

                // Return ack
                server.Udp.SendMessage(new AckErrorPacket(PacketTypes.LogoutAck, "Logged out correctly.").Pack(), ip);
            }
            else
            {
                server.DebugInfo("Logout: Incorrect logut.");
                // Send error
                message = "Logout error: " + message;
                server.Udp.SendError(message, ip);
            }
        }

        public static void ProfileUpdate(byte[] data, IPEndPoint ip, Server server)
        {
            string message = "";

            ProfilePacket pPacket = Packet.Unpack<ProfilePacket>(data);

            server.DebugInfo("Update profile recieved.");
            server.DebugInfo("Update profile Packet: " + pPacket.ToString());

            ClientStatus current = server.GetClient(pPacket.Username);

            if(ClientsManagement.UpdateProfile(pPacket, ref current, out message))
            {
                // Profile updated
                server.DebugInfo("Update Profile: " + pPacket.Username + "'s profile updated.");

                // Return user profile with the ack
                server.Udp.SendMessage(new ProfilePacket(
                                            PacketTypes.ProfileUpdateAck,
                                            current.Alea, // New alea generated
                                            current.Client.FirstName,
                                            current.Client.LastName,
                                            current.Client.Age,
                                            current.Client.PhoneNumber,
                                            current.Client.Gender,
                                            current.Client.Username,
                                            current.Client.Password,
                                            current.Client.Email,
                                            server.GetContacts(current),
                                            new List<Models.Message>(),
                                            server.GetAgendaEvents(current.Client.Username)
                                            ).Pack(), ip);
            }
            else
            {
                server.DebugInfo("Profile Update: Request not accepted");
                server.Udp.SendError(message, ip);
            }
        }

        public static void DeleteAccount(byte[] data, IPEndPoint ip, Server server)
        {
            string message = "";

            BasicReqPacket delPacket = Packet.Unpack<BasicReqPacket>(data);

            server.DebugInfo("Delete account request recieved.");
            server.DebugInfo("DeleteAccountReq Packet: " + delPacket.ToString());

            ClientStatus current = server.GetClient(delPacket.Username);

            if (ClientsManagement.CheckBasics(current, ClientStatus.Status.Disconnected, delPacket.Alea, out message))
            {
                server.DebugInfo("Delete Account: Account was " + delPacket.Username + " correctly deleted.");

                // Send the ack first
                server.Udp.SendMessage(new AckErrorPacket(PacketTypes.DeleteAccountAck, "Account deleted correctly.").Pack(), ip);

                // Delete client from database
                server.DeleteCLientFromDataBase(delPacket.Username);
            }
            else
            {
                server.DebugInfo("Delete account: Acount can't be deleted.");
                // Send error
                message = "Delete error: " + message;
                server.Udp.SendError(message, ip);
            }
        }
    }
}
