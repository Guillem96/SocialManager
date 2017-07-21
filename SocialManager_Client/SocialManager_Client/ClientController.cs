using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client
{
    static class ClientController
    {
        public static Client client; //< Reference to current client

        public static bool Register(Profile newProfile, out string message)
        { 
            return client.Register(newProfile, out message);
        }

        public static bool Login(string username, string password, out string message)
        {
            return client.Login(username, password, out message);
        }

        public static bool GetContactRequests(out string message)
        {
            return client.GetContactRequestList(out message);
        }

        public static bool ClientQuery(string query, ref List<Profile>usernames, out string message)
        {
            return client.ClientsQuery(query, out message, ref usernames);
        }

        public static bool SendContactRequest(string username, out string message)
        {
            return client.SendContactRequest(username, out message);
        }

        public static bool AcceptRequest(string username, out string message)
        {
            return client.AnswerContactRequest(
                                client.Profile.RecievedContactRequests
                                            .Single(c => c.From.Username == username),
                                true,
                                out message);
        }

        public static bool DenyRequest(string username, out string message)
        {
            return client.AnswerContactRequest(
                                client.Profile.RecievedContactRequests
                                            .Single(c => c.From.Username == username),
                                false,
                                out message);
        }

        public static bool AnyMessageFrom(string username)
        {
            return client.Profile.Messages.Any(m => m.From.Username == username);
        }

    }
}
