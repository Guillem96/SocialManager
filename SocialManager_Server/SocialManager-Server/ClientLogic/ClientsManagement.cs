using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
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
        // Resgister a new client to database
        public static bool RegisterClient(Packets.ProfilePacket packet, 
                                            IPEndPoint ip, 
                                            ref Models.Client c, 
                                            out string message)
        {
            try
            {
                // Some checks before registering the user
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

        // Client logs in 
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

        public static bool UpdateProfile(Packets.ProfilePacket packet, ref ClientStatus current, out string message)
        {
            try
            {
                if (CheckBasics(current, ClientStatus.Status.Disconnected, packet.Alea, out message))
                {
                    using (var db = new Models.ServerDatabase())
                    {
                        // Check fields that can be changed
                        Models.Client c = db.Clients.Single(r => r.Username == packet.Username);

                        if(c.FirstName != packet.FirstName)
                        {
                            message = "Update Profile Error: First Name can't be updated";
                            return false;
                        }

                        if (c.LastName != packet.LastName)
                        {
                            message = "Update Profile Error: Last Name can't be updated";
                            return false;
                        }

                        if (c.Age != packet.Age)
                        {
                            message = "Update Profile Error: Age can't be updated";
                            return false;
                        }

                        if (c.Genre != packet.Genre)
                        {
                            message = "Update Profile Error: Genre can't be updated";
                            return false;
                        }

                        // update the profile
                        c.Password = packet.Password;
                        c.PhoneNumber = packet.PhoneNumber;
                        c.Email = packet.Email;

                        // Set new reference to current
                        current.Client = c;

                        db.SubmitChanges();
                    }
   
                    message = "Cool!";
                    return true;
                }
                else
                {
                    message = "Update Profile Error: " + message;
                    return false;
                }
            }
            catch (SqlException e)
            {
                message = e.ToString();
                return false;
            }
        }

        // Get contact requests list for user current
        public static bool ContactRequestsList(Packets.BasicReqPacket packet, 
                                                ClientStatus current, 
                                                ref List<Models.ContactRequest> sent,
                                                ref List<Models.ContactRequest> recieved,
                                                out string message)
        {
            try
            {
                if (CheckBasics(current, ClientStatus.Status.Disconnected, packet.Alea, out message))
                {
                    var db = new Models.ServerDatabase();
                    
                    // Fill the lists with the requests 
                    recieved = db.ContactRequests
                                .Where( r => r.To.Username == current.Client.Username)
                                .ToList();

                    sent = db.ContactRequests
                                .Where(r => r.From.Username == current.Client.Username)
                                .ToList();

                    message = "Cool!";
                    return true;
                }
                else
                {
                    message = "Contact Requests List Error: " + message;
                    return false;
                }
            }
            catch (SqlException)
            {
                message = "Contact Requests List Error: Database error.";
                return false;
            }

        }

        // Get clients query result
        public static bool GetClientsQueryResult(Packets.ClientQueryPacket packet,
                                                ClientStatus current,
                                                ref List<string> result,
                                                string query,
                                                out string message)
        {
            try
            {
                if (CheckBasics(current, ClientStatus.Status.Disconnected, packet.Alea, out message))
                {
                    var db = new Models.ServerDatabase();

                    // Fill the lists with the requests 
                    result = db.Clients
                                .Where(r => r.Username.Contains(query) ||
                                            r.FirstName.Contains(query) ||
                                            r.LastName.Contains(query))
                                .Select(r => r.Username)
                                .ToList();
                                

                    message = "Cool!";
                    return true;
                }
                else
                {
                    message = "Client query List Error: " + message;
                    return false;
                }
            }
            catch (SqlException)
            {
                message = "Client query List Error: Database error.";
                return false;
            }

        }

        public static bool NewContactRequest(Packets.ContactReqPacket packet,
                                             ClientStatus current,
                                             out string message)
        {
            try
            {
                if (CheckBasics(current, ClientStatus.Status.Disconnected, packet.Alea, out message))
                {
                    using (var db = new Models.ServerDatabase())
                    {

                        // Check if destination exists
                        if (!db.Clients.Any(c => c.Username == packet.To))
                        {
                            message = "Destination doesn't exist.";
                            return false;
                        }

                        // Check if contact exists
                        if (db.Contacts.Any(c => (c.Client1.Username == packet.To && c.Client2.Username == packet.From) ||
                                                    (c.Client1.Username == packet.From && c.Client2.Username == packet.To)))
                        {
                            message = "Contact already exists.";
                            return false;
                        }

                        // Check if requests exists on any of both sides
                        if (db.ContactRequests.Any(c => (c.From.Username == packet.To && c.To.Username == packet.From) ||
                                                    (c.From.Username == packet.From && c.To.Username == packet.To)))
                        {
                            message = "Contact request already exists.";
                            return false;
                        }

                        // Create new record to ContactRequests Table
                        db.ContactRequests.InsertOnSubmit(new Models.ContactRequest()
                        {
                            From = db.Clients.Single(c => c.Username == packet.From),
                            To = db.Clients.Single(c => c.Username == packet.To)
                        });

                        db.SubmitChanges();
                    }
                }
                else
                {

                    return false;
                }
                message = "Cool!";
                return true;
            }
            catch (SqlException)
            {
                message = "New contact request error: Database error";
                return false;
            }  
        }

        public static bool AckOrRegContactReq(Packets.ContactReqPacket packet,
                                             ClientStatus current,
                                             bool ack,
                                             out string message)
        {
            try
            {
                if (CheckBasics(current, ClientStatus.Status.Disconnected, packet.Alea, out message))
                {
                    using (var db = new Models.ServerDatabase())
                    {

                        // Check if requests exists on any of both sides
                        if (!db.ContactRequests.Any(c => (c.From.Username == packet.From && c.To.Username == packet.To)))
                        {
                            message = "Contact request doesn't exists.";
                            return false;
                        }

                        // Remove the contact request
                        db.ContactRequests.DeleteOnSubmit(
                            db.ContactRequests
                            .Single(r => r.From.Username == packet.From && r.To.Username == packet.To)
                        );

                        if (ack)
                        {
                            // Create new record to Contact Table
                            db.Contacts.InsertOnSubmit(new Models.Contact()
                            {
                                Client1 = db.Clients.Single(c => c.Username == packet.From),
                                Client2 = db.Clients.Single(c => c.Username == packet.To)
                            });
                            message = "Contact added correctly.";
                        }
                        else
                        {
                            message = "Contact request refused correctly.";
                        }

                        db.SubmitChanges();
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (SqlException)
            {
                message = "New contact request error: Database error";
                return false;
            }
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
