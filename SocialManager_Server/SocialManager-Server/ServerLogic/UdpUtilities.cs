using System;
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

                using (var db = new Models.ServerDatabase())
                {
                    List<string> contactsUsername = 
                                    db.Contacts
                                        .Where(c => c.Client1.Username == current.Username || c.Client2.Username == current.Username)
                                        .Select(c => c.Client1.Username == current.Username ? c.Client2.Username : c.Client1.Username).ToList();

                    List<ClientStatus> contacts = contactsUsername.Select(c => server.GetClient(c)).ToList();
                    
                    // Return user profile with the ack
                    server.Udp.SendMessage(new Packets.ProfilePacket(
                                                Packets.PacketTypes.LoginAck,
                                                alea, // New alea generated
                                                current.FirstName,
                                                current.LastName,
                                                current.Age,
                                                current.PhoneNumber,
                                                current.Genre,
                                                current.Username,
                                                current.Password,
                                                contacts
                                            ).Pack(), ip);
                }
                
            }
            else
            {
                server.DebugInfo("Login: " + logPacket.Username + " request not accepted");
                server.Udp.SendError(message, ip);
            }
        }

        public static void Logout(byte[] data, IPEndPoint ip, Server server)
        {
            string message = "";

            // Use alive packet because contains the same info as a logoutReq packet.
            BasicReqPacket logoutPacket = Packet.Unpack<BasicReqPacket>(data);

            server.DebugInfo("Logut request recieved.");
            server.DebugInfo("LogoutReq Packet: " + logoutPacket.ToString());

            ClientStatus current = server.GetClient(logoutPacket.Username);

            if(ClientsManagement.CheckBasics(current,ClientStatus.Status.Disconnected, logoutPacket.Alea, out message))
            {
                server.DebugInfo("Logout: Correct logut from " + logoutPacket.Username);
                server.DebugInfo(logoutPacket.Username + " now is disconnected.");
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

        public static void DeleteAccount(byte[] data, IPEndPoint ip, Server server)
        {
            string message = "";
           

            // Use alive packet because contains the same info as a DeleteAccountReq packet.
            BasicReqPacket delPacket = Packet.Unpack<BasicReqPacket>(data);

            server.DebugInfo("Delete account request recieved.");
            server.DebugInfo("DeleteAccountReq Packet: " + delPacket.ToString());

            ClientStatus current = server.Clients
                                        .Any(c => c.Client.Username == delPacket.Username)
                                        ? server.GetClient(delPacket.Username)
                                        : null;

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

        public static void Alive(byte[] data, IPEndPoint ip, Server server)
        {
            string message = "";

            BasicReqPacket aPacket = Packet.Unpack<BasicReqPacket>(data);

            server.DebugInfo("Alive inf recieved.");
            server.DebugInfo("AliveInf Packet: " + aPacket.ToString());

            ClientStatus current = server.GetClient(aPacket.Username);

            if (ClientsManagement.CheckBasics(current, ClientStatus.Status.Disconnected, aPacket.Alea, out message))
            {
                // Save the last alive
                current.LastAlive = DateTime.Now;
                // Send ack
                server.Udp.SendMessage(new AckErrorPacket(PacketTypes.AliveAck, "Alive correct").Pack(), ip);
            }
            else
            {
                server.DebugInfo("Alive: Incorrect alive from " + aPacket.Username);
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

        public static void SendContactRequests(byte[] data, Server server, IPEndPoint ip)
        {
            string message = "";

            // Unpack the petition
            BasicReqPacket cPacket = Packet.Unpack<BasicReqPacket>(data);

            List<Models.ContactRequest> requests = null;

            server.DebugInfo("Contact requests list requested by " + cPacket.Username);
            server.DebugInfo(cPacket.ToString());

            if (ClientsManagement.ContactRequestsList(cPacket,
                                                        server.GetClient(cPacket.Username),
                                                        ref requests,
                                                        out message))
            {

                // List filled correctly
                server.Udp.SendMessage(new ContactReqListPacket(PacketTypes.ContactAck,
                                                                        cPacket.Alea,
                                                                        requests).Pack(),
                                                                        ip);

                server.DebugInfo("Contact list requests sended correctly to + " + cPacket.Username);
            }
            else
            {
                message = "Contact requests list error: " + message;
                server.DebugInfo(message);
                server.Udp.SendError(message, ip);
            }
        }
    }
}
