using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.ClientLogic
{
    /// <summary>
    /// Manage client requests
    /// </summary>
    static class ClientsManagement
    {
        public static bool RegisterClient(Packets.ProfilePacket packet, IPEndPoint ip, Models.Client c, out string message)
        {
            try
            {
                // Do checks before registering the user
                if (!packet.Alea.Equals("0000000"))
                {
                    message = "Alea must be 7 0s";
                    return false;
                }

                // Declaring the new client
                c = new Models.Client()
                {
                    FirstName = packet.FirstName,
                    LastName = packet.LastName,
                    Age = packet.Age,
                    PhoneNumber = packet.PhoneNumber,
                    Genre = packet.Genre,
                    Username = packet.Username,
                    Password = packet.Password,
                    Email = packet.Email,
                    Ip = ip.Address.GetAddressBytes()
                };

                // Open database and check if client already exists
                using (Models.ServerDatabase db = new Models.ServerDatabase())
                {
                    if (db.Clients.Where(a => a.Username == packet.Username).Count() != 0)
                    {
                        message = "Username " + c.Username + " already exists.";
                        return false;
                    }
                    db.Clients.InsertOnSubmit(c);
                    db.SubmitChanges();
                }
                message = "Cool!";
                return true;
            }
            catch (Exception e)
            {
                message = "Exception thrown: " + e.ToString();
                return false;
            }
            
        }

        public static bool LoginClient(Packets.LoginReqPacket packet, ref Models.Client c, out string message)
        {
            try
            {
                using(Models.ServerDatabase db = new Models.ServerDatabase())
                {
                    var cli = db.Clients.Where(a => a.Username == packet.Username);
                    // If user exists
                    if (cli.Count() == 0)
                    {
                        message = "No user with " + packet.Username + " username in the database.";
                        return false;
                    }

                    c = cli.First();

                    // if password is correct
                    if (c.Password != packet.Password)
                    {
                        message = "Incorrect password.";
                        return false;
                    }
                    message = "Cool!";
                    return true;
                }
            }
            catch (Exception)
            {
                message = "Unexpected error.";
                return false;
            }
        }

        public static bool AliveClient(Packets.AlivePacket packet, ClientStatus current, out string message)
        {
            bool res = CheckBasics(current, ClientStatus.Status.Disconnected, packet.Alea, out message);

            if (!res)
            {
                message = "AliveInf Error: " + message;
            }

            return res;
        }

        /// <summary>
        /// Checks if user is prepared for his request.
        /// </summary>
        /// <param name="current">Client who is making the request.</param>
        /// <param name="equalsTo">Status which the client could not be.</param>
        /// <param name="alea">Packet alea number.</param>
        public static bool CheckBasics(ClientStatus current, 
                                        ClientStatus.Status equalsTo, 
                                        string alea,
                                        out string message)
        {
            // No user in database
            if (current == null)
            {
                message = "User not found in database.";
                return false;
            }

            // Trying to send alive when disconnected
            if (current.Stat == equalsTo)
            {
                message = "User is disconnected.";
                return false;
            }

            // Incorrect alea number
            if (!alea.Equals(current.Alea))
            {
                message = "Incorrect alea number.";
                return false;
            }

            message = "Cool!";
            return true;
        }
    }
}
