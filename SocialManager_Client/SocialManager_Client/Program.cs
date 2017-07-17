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

        static bool Login(Client c)
        {
            string username = Ask("Username: ");
            string password = Ask("Password: ");

            string message = "";
            bool res = c.Login(username, password, out message);
            Console.WriteLine(message);
            return res;
        }

        static void AddContactMenu(Client c)
        {
            string message = "";
            // Until Logout
            while (true)
            {
                int op = int.Parse(Ask("1. Exit" + Environment.NewLine +
                                        "2. Show Contacts Requests" + Environment.NewLine +
                                        "3. New Contact Requests (Not implemented)" + Environment.NewLine +
                                        "Option: "));

                switch (op)
                {
                    case 1:
                        Console.WriteLine("------------------------------");
                        return;
                    case 2:
                        c.GetContactRequestList(out message);
                        Console.WriteLine(String.Join(Environment.NewLine + "- ", c.Profile.ContactRequests.Select(r => r.From.Username).ToList()));
                        break;
                    case 3:
                        break;
                    
                    default:
                        Console.WriteLine("Unexpected option.");
                        break;
                }
            }
        }

        static void Alive(Client c)
        {
            string message = "";
            // Until Logout
            while (true)
            {
                int op = int.Parse(Ask("1. Logout" + Environment.NewLine +
                                        "2. Show Profile" + Environment.NewLine +
                                        "3. Add Contact" + Environment.NewLine +
                                        "4. Send Message (Not implemented)" + Environment.NewLine +
                                        "5. Remove acount." + Environment.NewLine +
                                        "Option: "));

                switch (op)
                {
                    case 1:
                        if(c.Logout(out message))
                        {
                            return;
                        }
                        else
                        {
                            Console.WriteLine(message);
                        }

                        break;
                    case 2:
                        Console.WriteLine(c.Profile.ToString());
                        break;
                    case 3:
                        AddContactMenu(c);
                        break;
                    case 4:
                        break;
                    case 5:
                        if(c.DeleteAccount(out message))
                        {
                            return;
                        }
                        break;
                    default:
                        Console.WriteLine("Unexpected option.");
                        break;
                }
            }
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
                        if (Login(c))
                        {
                            Alive(c);
                        }
                        break;
                    case 3:
                        return;
                }
            }
        }
    }
}
