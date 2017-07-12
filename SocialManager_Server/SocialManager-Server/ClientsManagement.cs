using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server
{
    static class ClientsManagement
    {
        public static bool RegisterClient(Packets.RegisterReqPacket packet)
        {
            // Do checks before registering the user
            if (!packet.Alea.Equals("0000000"))
            {
                Console.WriteLine("Alea error.");
                return false;
            }

            // Adding the new client to database
            Models.ServerDatabase db = new Models.ServerDatabase();
            Models.Client c = new Models.Client()
            {
                FirstName = packet.FirstName,
                LastName = packet.LastName,
                Age = packet.Age,
                PhoneNumber = packet.PhoneNumber,
                Genre = packet.Genre,
                Username = packet.Username,
                Password = packet.Password,
                Email = packet.Email
            };

            if (db.Clients.Where(a => a.Username == c.Username).Count() != 0)
            {
                Console.WriteLine("Client is already registered.");
                return false;
            }

            db.Clients.InsertOnSubmit(c);
            db.SubmitChanges();
            return true;
        }
    }
}
