using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstagramGot;
using InstagramGot.Auth;
using System.Text.RegularExpressions;
using OpenQA.Selenium.PhantomJS;
using InstagramGot.Models;
using InstagramGot.Exceptions;
using InstagramGot.Parameters;

namespace SocialManager_Client.SocialNetworksLogic
{
    class Instagram
    {
        // User information
        private string username;
        private string password;
        private bool loggedIn;

        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public bool LoggedIn { get => loggedIn; set => loggedIn = value; }


        // App credentials
        private string clientID = "6a5985a97e0842d3a97c80d29a761f83";
        private IAuthContext authContext;

        public Instagram(string username, string password)
        {
            Username = username;
            Password = password;
            LoggedIn = false;

            // Set the app auth
            var credentials = new InstagramCredentials(clientID);
            authContext = AuthFlow.InitAuth(credentials, "https://github.com/Guillem96");
        }

        public bool Login()
        {
            
            if (LoggedIn) return true;

            string token = GetAccessToken(authContext.AuthorizationURL);

            if (token == "") return false;

            // Set the credentials
            AuthFlow.CreateCredentialsFromAccesToken(token);

            AuthFlow.SetUserCredentials();


            LoggedIn = true;
            return true;
            
        }

        public void Logout()
        {
            LoggedIn = false;
        }

        private string GetAccessToken(string url)
        {
            try
            { 
                var driverService = PhantomJSDriverService.CreateDefaultService();

                driverService.HideCommandPromptWindow = true;

                driverService.LoadImages = false;
                driverService.SslProtocol = "tlsv1";
                driverService.IgnoreSslErrors = true;
                driverService.ProxyType = "https";
                driverService.Proxy = "";

                using (var phantom = new PhantomJSDriver(driverService))
                {
                    // Delete the cookies
                    phantom.Manage().Cookies.DeleteAllCookies();
                    // Navigate to Url of authentification
                    phantom.Navigate().GoToUrl(url);

                    // Fill the login fields
                    var userField = phantom.FindElementById("id_username");
                    var passField = phantom.FindElementById("id_password");
                    var loginButton = phantom.FindElementByClassName("button-green");

                    userField.SendKeys(username);
                    passField.SendKeys(password);
                    loginButton.Click();

                    // Get the acces token
                    Regex regex = new Regex(@"access_token=\S+$");
                    var match = regex.Match(phantom.Url);

                    if (match.Success)
                    {
                        return match.Value.Substring("access_token=".Length);
                    }

                    return "";
                }
            }
            catch (OpenQA.Selenium.NoSuchElementException)
            {
                return "";
            }

}


        public IUser GetUserById(long id)
        {
            try
            {
                return UserManager.GetUser(id);

            }
            catch (InstagramAPICallException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get profile image url
        /// </summary>
        public string GetProfileImageUrl()
        {
            return UserManager.GetAuthenticatedUser().ProfilePictureUrl;
        }

        /// <summary>
        /// Get recent media of authenticated user
        /// </summary>
        /// <param name="count">Default 10</param>
        /// <returns></returns>
        public List<IMedia> GetRecentMedia(int count = 10)
        {
            try
            {
                return UserManager.GetRecentMedia(count);
            }
            catch (InstagramAPICallException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get recent media of given user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="count">Default 2</param>
        /// <returns></returns>
        public List<IMedia> GetRecentMedia(long userId, int count = 2)
        {
            try
            {
                return UserManager.GetRecentMedia(new UsersQueryParameters()
                {
                    Count = count,
                    Id = userId
                });
            }
            catch (InstagramAPICallException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get follows
        /// </summary>
        /// <param name="count">Default 20 follows</param>
        /// <returns></returns>
        public List<IMinifiedUser> GetFollows(int count = 20)
        {
            try
            {
                return RelationshipsManager.GetFollows().Take(count).ToList();
            }
            catch (InstagramAPICallException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get followers
        /// </summary>
        /// <param name="count">Default 20 followers</param>
        /// <returns></returns>
        public List<IMinifiedUser> GetFollowedBy(int count = 20)
        {
            try
            {
                return RelationshipsManager.GetFollowedBy().Take(count).ToList();
            }
            catch (InstagramAPICallException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get requests
        /// </summary>
        /// <param name="count">Default 20 requests</param>
        /// <returns></returns>
        public List<IMinifiedUser> GetRequestedBy(int count = 20)
        {
            try
            {
                return RelationshipsManager.GetRequestedBy().Take(count).ToList();
            }
            catch (InstagramAPICallException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get comments from given media
        /// </summary>
        public List<IComment> GetComments(string mediaId, int count = 20)
        {
            try
            {
                return CommentsManager.GetCommentsFromMedia(mediaId).Take(count).ToList();
            }
            catch (InstagramAPICallException)
            {
                return null;
            }
        }

        /// <summary>
        /// Post a comment
        /// </summary>
        public bool PostComment(string mediaId, string text)
        {
            try
            {
                return CommentsManager.PostCommentToMedia(mediaId, text);
            }
            catch (InstagramAPICallException)
            {
                return false;
            }
            catch(CommentFormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Delete a comment
        /// </summary>
        public bool DeleteComment(string mediaId, long commentId)
        {
            try
            {
                return CommentsManager.DeleteComment(mediaId, commentId);
            }
            catch (InstagramAPICallException)
            {
                return false;
            }
        }

        /// <summary>
        /// Start to follow a user
        /// </summary>
        public bool Follow(long userId)
        {
            try
            {
                RelationshipsManager.CreateRelationship(userId);
                return true;
            }
            catch (InstagramAPICallException)
            {
                return false;
            }
        }

        /// <summary>
        /// Unfollow a user
        /// </summary>
        public bool Unfollow(long userId)
        {
            try
            {
                RelationshipsManager.DestroyRelationship(userId);
                return true;
            }
            catch (InstagramAPICallException)
            {
                return false;
            }
        }

        /// <summary>
        /// Approve a request
        /// </summary>
        public bool Approve(long userId)
        {
            try
            {
                RelationshipsManager.ApproveRelationship(userId);
                return true;
            }
            catch (InstagramAPICallException)
            {
                return false;
            }
        }

        /// <summary>
        /// Decline a request
        /// </summary>
        public bool Ignore(long userId)
        {
            try
            {
                RelationshipsManager.IgnoreRelationship(userId);
                return true;
            }
            catch (InstagramAPICallException)
            {
                return false;
            }
        }
    }
}
