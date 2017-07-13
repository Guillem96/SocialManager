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
        private static string Ask(string msg)
        {
            Console.Write(msg);
            return Console.ReadLine();
        }

        static Profile AskProfile()
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
            return p;
        }

        static void Main(string[] args)
        {
            

            //Profile p = new Profile("Guillem", "Orellana", 21, "600886706", Profile.Sex.Male, "Guillem96", "1234", "guillem.orellana@gmail.com");

            Client c = new Client();

            string msg;

            c.Register(AskProfile(), out msg);

            Console.WriteLine(msg);

            Console.Read();
        }
    }
}
