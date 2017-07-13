﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server
{
    /// <summary>
    /// Manage client requests
    /// </summary>
    static class ClientsManagement
    {
        public static bool RegisterClient(Packets.ProfilePacket packet, IPEndPoint ip, out string message)
        {
            try
            {
                // Do checks before registering the user
                if (!packet.Alea.Equals("0000000"))
                {
                    Console.WriteLine("Alea error.");
                    message = "Alea must be 7 0s";
                    return false;
                }

                // Declaring the new client
                Models.Client c = new Models.Client()
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
                    if (db.Clients.Where(a => a.Username == c.Username).Count() != 0)
                    {
                        Console.WriteLine("Client is already registered.");
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
                    c = db.Clients.Where(a => a.Username == packet.Username).First();
                    // If user exists
                    if (c == null)
                    {
                        message = "No user with " + packet.Username + " username in the database.";
                        return false;
                    }
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
    }
}
