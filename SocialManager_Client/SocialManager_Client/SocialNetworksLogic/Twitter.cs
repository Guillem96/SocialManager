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

        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }


        // App credentials
        private string consumerKey = "LotV1wCnHHlZml9Os0z9ZJeJq";
        private string consumerSecretKey = "FfsqM4ieMT5PLCmk92rmP8kcYPGzaA9puDAYouK0p3HurobCCo";

        private IAuthenticationContext authenticationContext;


        public Twitter(string username, string password)
        {
            // User information
            this.Username = username;
            this.Password = password;

            // Set the app auth
            var credentials = new TwitterCredentials(consumerKey, consumerSecretKey);
            authenticationContext = AuthFlow.InitAuthentication(credentials);
        }

        public bool Login()
        {
            string pin = GetPinCode();

            if (pin == "") return false;

            // With this pin code it is now possible to get the credentials back from Twitter
            var userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pin, authenticationContext);

            // Use the user credentials in your application
            Auth.SetCredentials(userCredentials);

            return true;
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

                    Console.WriteLine(phantom.Url);
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


        public ITweet ReplyTweet(long tweetId, string text)
        {
            var tweetToReplyTo = Tweet.GetTweet(tweetId);

            // We must add @screenName of the author of the tweet we want to reply to
            var textToPublish = string.Format("@{0} {1}", tweetToReplyTo.CreatedBy.ScreenName, text);

            return Tweet.PublishTweetInReplyTo(textToPublish, tweetId);
        }


        public ITweet Retweet(long id) { return Tweet.PublishRetweet(id); }

        public bool FavoriteTweet(long id) { return Tweet.FavoriteTweet(id); }

        public bool DeleteTweet(long id) { return Tweet.DestroyTweet(id); }


        public void GetNewTweets(int many)
        {

        }

        public IEnumerable<ITweet> GetProfileTweets(int many)
        {
            return Timeline.GetUserTimeline(User.GetAuthenticatedUser().Id, many);
        }

       
        public string GetProfileImage()
        {
            return User.GetAuthenticatedUser().ProfileImageUrl400x400;
        }

        public string GetProfileImage(long userID)
        {
            return User.GetUserFromId(userID).ProfileImageUrl;
        }
    }
}
