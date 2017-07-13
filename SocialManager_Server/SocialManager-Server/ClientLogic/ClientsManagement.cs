using System;
using System.Collections.Generic;
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
        public static bool RegisterClient(Packets.ProfilePacket packet, IPEndPoint ip, ref Models.Client c, out string message)
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
                    if(c.Password != packet.Password)
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
            // No user in database
            if (current == null)
            {
                message = "AliveInf: User not found in database.";
                return false;
            }

            // Trying to send alive when disconnected
            if (current.Stat == ClientStatus.Status.Disconnected)
            {
                message = "AliveInf: User is disconnected.";
                return false;
            }

            // Incorrect alea number
            if (!packet.Alea.Equals(current.Alea))
            {
                message = "AliveInf: Incorrect alea number.";
                return false;
            }

            message = "Cool!";
            return true;
        }
    }
}
