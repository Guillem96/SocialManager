using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client
{
    class Program
    {
        // This part won't exist in a few days, it's here only for testing
        private static string Ask(string msg)
        {
            Console.Write(msg);
            return Console.ReadLine();
        }

        static void Register(Client c)
        {
            Profile p = new Profile();
            p.FirstName = Ask("First name: ");
            p.LastName = Ask("Last name: ");
            p.Age = int.Parse(Ask("Age: "));
            p.Genre = Ask("Genre (M o F): ").Equals("M") ? Profile.Sex.Male : Profile.Sex.Female;
            p.Email = Ask("Email: ");
            p.PhoneNumber = Ask("Phone number: ");
            p.Username = Ask("Usrname: ");
            p.Password = Ask("Password: ");

            string message = "";

            c.Register(p, out message);

            Console.WriteLine(message);
        }

        static void Login(Client c)
        {
            string username = Ask("Username: ");
            string password = Ask("Password: ");

            string message = "";
            c.Login(username, password, out message);
            Console.WriteLine(message);
        }

        static void Main(string[] args)
        {
            
            Client c = new Client();

            while (true)
            {
                int op = int.Parse(Ask("1. Register" + Environment.NewLine +
                                        "2. Login" + Environment.NewLine +
                                        "3. Exit" + Environment.NewLine +
                                        "Option: "));

                switch (op)
                {
                    case 1:
                        Register(c);
                        break;
                    case 2:
                        Login(c);
                        break;
                    case 3:
                        return;
                }
            }
            
            Register(c);
            
            Console.Read();
        }
    }
}
