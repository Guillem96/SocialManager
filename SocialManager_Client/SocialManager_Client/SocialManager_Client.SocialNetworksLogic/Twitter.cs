﻿
using OpenQA.Selenium.PhantomJS;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace SocialManager_Client.SocialNetworksLogic
{
    class Twitter
    {
        // User information
        private string username;
        private string password;
        private bool loggedIn;

        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public bool LoggedIn { get => loggedIn; set => loggedIn = value; }


        // App credentials
        private string consumerKey = "LotV1wCnHHlZml9Os0z9ZJeJq";
        private string consumerSecretKey = "FfsqM4ieMT5PLCmk92rmP8kcYPGzaA9puDAYouK0p3HurobCCo";

        private IAuthenticationContext authenticationContext;


        public Twitter(string username, string password)
        {
            // User information
            this.Username = username;
            this.Password = password;
            LoggedIn = false;

            // Set the app auth
            var credentials = new TwitterCredentials(consumerKey, consumerSecretKey);
            authenticationContext = AuthFlow.InitAuthentication(credentials);
        }

        public bool Login()
        {
            if (LoggedIn) return true;

            string pin = GetPinCode();

            if (pin == "") return false;

            // With this pin code it is now possible to get the credentials back from Twitter
            var userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pin, authenticationContext);

            // Use the user credentials in your application
            Auth.SetCredentials(userCredentials);

            LoggedIn = true;
            return true;
        }

        public void Logout()
        {
            LoggedIn = false;
        }

        // Get the pin code using web scraping
        private string GetPinCode()
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
                    phantom.Manage().Cookies.DeleteAllCookies();

                    phantom.Navigate().GoToUrl(authenticationContext.AuthorizationURL);

                    var userField = phantom.FindElementById("username_or_email");
                    var passField = phantom.FindElementById("password");
                    var loginButton = phantom.FindElementById("allow");

                    userField.SendKeys(Username);
                    passField.SendKeys(Password);
                    loginButton.Click();


                    var key = phantom.FindElementByTagName("code").Text;

                    phantom.Quit();
                    return key;
                }
            }
            catch (OpenQA.Selenium.NoSuchElementException)
            {
                return "";
            }   
        }


        public ITweet PublishTweet(string text, string mediaPath = "")
        {
            if (mediaPath != "")
            {
                byte[] bytes = File.ReadAllBytes(mediaPath);
                var media = Upload.UploadImage(bytes);

                return Tweet.PublishTweet(text, new PublishTweetOptionalParameters
                {
                    Medias = new List<IMedia> { media }
                });
            }
            else
            {
                return Tweet.PublishTweet(text);
            }
        }

        internal IUser GetUserByScreenName(string text)
        {
            return User.GetUserFromScreenName(text);
        }

        public ITweet ReplyTweet(long tweetId, string text)
        {
            var tweetToReplyTo = Tweet.GetTweet(tweetId);

            // We must add @screenName of the author of the tweet we want to reply to
            var textToPublish = string.Format("@{0} {1}", tweetToReplyTo.CreatedBy.ScreenName, text);

            return Tweet.PublishTweetInReplyTo(textToPublish, tweetId);
        }

        internal string GetBannerImageUrl()
        {
            return User.GetAuthenticatedUser().ProfileBannerURL;
        }

        public ITweet Retweet(long id) { return Tweet.PublishRetweet(id); }

        public bool FavoriteTweet(long id) { return Tweet.FavoriteTweet(id); }

        public bool DeleteTweet(long id) { return Tweet.DestroyTweet(id); }

        public IEnumerable<ITweet> GetProfileTweets(int many)
        {
            return Timeline.GetUserTimeline(User.GetAuthenticatedUser().Id, many);
        }

        public IEnumerable<ITweet> GetHomeTweets(int many)
        {
            return Timeline.GetHomeTimeline(many);
        }

        public string GetProfileImage()
        {
            return User.GetAuthenticatedUser().ProfileImageUrl400x400;
        }

        public string GetProfileImage(long userID)
        {
            return User.GetUserFromId(userID).ProfileImageUrl;
        }

        public IEnumerable<IUser> GetFollowers()
        {
            return User.GetFollowers(User.GetAuthenticatedUser().Id);
        }

        public IEnumerable<IUser> GetFriends()
        {
            return User.GetFriends(User.GetAuthenticatedUser().Id);
        }

        public IUser GetUserById(long id)
        {
            return User.GetUserFromId(id);
        }

        public IRelationshipDetails GetFriendShipWith(long id)
        {
            return Friendship.GetRelationshipDetailsBetween(User.GetAuthenticatedUser().Id, id);
        }

        public bool Unfollow(IUser user)
        {
            return Friendship.FriendshipController.DestroyFriendshipWith(user.UserIdentifier);
        }

        public bool Follow(IUser user)
        {
            return Friendship.FriendshipController.CreateFriendshipWith(user.UserIdentifier);
        }

        public List<IUser> SearchUsers(string text)
        {
            var parameters = new SearchUsersParameters(text)
            {
                MaximumNumberOfResults = 10,
                
            };
            var res = Search.SearchUsers(text, 10);
            return res?.ToList();
        }
    }
}
