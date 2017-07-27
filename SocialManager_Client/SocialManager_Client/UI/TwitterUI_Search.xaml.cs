using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tweetinvi.Models;

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for TwitterUI_Search.xaml
    /// </summary>
    public partial class TwitterUI_Search : UserControl
    {
        private SocialNetworksLogic.Twitter twitter;
        private string userLoaded = "";
        private string prevQuery = "";

        // Waiting for search
        private double loadingWaitForUserWidth;
        private double loadingWaitForUserHeight;

        // Loading tweets greed
        private double loadingTweetsGridWidth;
        private double loadingTweetsHeight;

        // Loading profile image greed
        private double loadingImageGridWidth;
        private double loadingImageHeight;

        // Loading profile info
        private double loadingInfoGridWidth;
        private double loadingInfoHeight;

        internal TwitterUI_Search(SocialNetworksLogic.Twitter twitter)
        {
            InitializeComponent();

            this.twitter = twitter;

            // Set the buttons
            FavButton.Content = new Image() { Source = PathUtilities.GetImageSource("heart.png") };
            RetButton.Content = new Image() { Source = PathUtilities.GetImageSource("retweet.png") };

            FavButton.IsEnabled = false;
            RetButton.IsEnabled = false;

            Follow.Content = new Image() { Source = PathUtilities.GetImageSource("add.png") };
            Unfollow.Content = new Image() { Source = PathUtilities.GetImageSource("delete.png") };

            Follow.IsEnabled = false;
            Unfollow.IsEnabled = false;

            // Set grid size
            loadingWaitForUserWidth = LoadingGridWaitForUser.Width;
            loadingWaitForUserHeight = LoadingGridWaitForUser.Height;

            loadingTweetsGridWidth = LoadingGridTweets.Width;
            loadingTweetsHeight = LoadingGridTweets.Height;

            loadingImageGridWidth = LoadingProfileImage.Width;
            loadingImageHeight = LoadingProfileImage.Height;

            loadingInfoGridWidth = LoadingInfoGrid.Width;
            loadingInfoHeight = LoadingInfoGrid.Height;

            // Load Data
            new Thread(new ThreadStart(() => LoadData())).Start();
        }

        // Autocomplete box
        private void UserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            new Thread(new ThreadStart(() =>
            {
                var suggestions = new List<string>();

                if (prevQuery.Length > 2)
                {
                    suggestions = twitter.SearchUsers(prevQuery).Select(u => u.ScreenName).ToList();

                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                    {
                        if (UserSearch.Text == "") return;

                        UsersSuggestion.ItemsSource = null;

                        if (suggestions.Count > 0)
                        {
                            UsersSuggestion.ItemsSource = suggestions;
                            UsersSuggestion.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            UsersSuggestion.ItemsSource = null;
                            UsersSuggestion.Visibility = Visibility.Collapsed;
                        }
                        prevQuery = UserSearch.Text;

                    }));
                }
                else
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                    {
                        if (UserSearch.Text == "") return;

                        UsersSuggestion.ItemsSource = null;
                        UsersSuggestion.Visibility = Visibility.Collapsed;
                        prevQuery = UserSearch.Text;

                    }));
                }
            })).Start();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            userLoaded = UserSearch.Text;
            UserSearch.Text = "";

            UsersSuggestion.ItemsSource = null;
            UsersSuggestion.Visibility = Visibility.Collapsed;

            prevQuery = "";

            // Load data
            Thread t = new Thread(new ThreadStart(() => LoadData()));
            t.Start();
            t.Join();
        }

        private void LoadData()
        {
            if(userLoaded == "")
            {
                // Waiting for user query
                Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingWaitForUserHeight, loadingWaitForUserWidth, LoadingGridWaitForUser, "Esperando a la búsqueda...")));
                return;
            }
            else
            {
                // End loading for user query
                Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGridWaitForUser)));

                // Start loading user's tweets
                Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingTweetsHeight, loadingTweetsGridWidth, LoadingGridTweets, "Cargando tweets...", 70, 70)));
                new Thread(new ThreadStart(() => 
                {
                    if (!LoadTweets())
                    {
                        MessageBox.Show("Error cargando los tweets.");
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGridTweets)));
                    }
                })).Start();

                // Start loading user's profile image
                Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingImageHeight, loadingImageGridWidth, LoadingProfileImage, "Cargando imagen...", 50, 50)));
                new Thread(new ThreadStart(() =>
                {
                    if (!LoadImage())
                    {
                        MessageBox.Show("Error cargando la imagen de perfil");
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingProfileImage)));
                    }
                })).Start();

                // Start loading user's profile image
                Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingInfoHeight, loadingInfoGridWidth, LoadingInfoGrid, "Cargando info...", 50, 50)));
                new Thread(new ThreadStart(() =>
                {
                    if (!LoadInfo())
                    {
                        MessageBox.Show("Error cargando la información del perfil.");
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingInfoGrid)));
                    }
                })).Start();
            }
        }

        private bool LoadImage()
        {
            try
            {
                List<TwitterUI_Tweets.TweetView> tweetList = new List<TwitterUI_Tweets.TweetView>();

                // Get the user
                IUser user = twitter.GetUserByScreenName("@" + userLoaded);

                if (user == null)
                    return false;

                string url = user.ProfileImageUrlFullSize;
                string urlBanner = user.ProfileBannerURL;

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                {
                    MainGrid.Background = new ImageBrush()
                    {
                        ImageSource = urlBanner != null ? PathUtilities.GetImageSourceFromUri(urlBanner) : null,
                        Stretch = Stretch.UniformToFill,
                        Opacity = 0.2
                    };
                    ProfileImage.Fill = new ImageBrush() { ImageSource = url == null ? null : PathUtilities.GetImageSourceFromUri(url) };    
                }));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Fill the tweets container
        private bool LoadTweets()
        {
            try
            {
                List<TwitterUI_Tweets.TweetView> tweetList = new List<TwitterUI_Tweets.TweetView>();

                // Get the user
                IUser user = twitter.GetUserByScreenName(userLoaded);

                if (user == null)
                    return false;

                // Get the user's tweets
                var tweets = user.GetUserTimeline(15);
                // Get profile image url 
                string url = user.ProfileImageUrlFullSize;

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                {
                    TweetsOfLabel.Content = "Tweets de " + user.ScreenName;
                    foreach (ITweet t in tweets)
                    {
                        tweetList.Add(new TwitterUI_Tweets.TweetView()
                        {
                            Tweet = t,
                            TweetText = t.Text,
                            UserImage = PathUtilities.GetImageSourceFromUri(url)
                        });
                    }
                    TweetsContainer.ItemsSource = null;
                    TweetsContainer.ItemsSource = tweetList;
                }));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool LoadInfo()
        {
            try
            {
                List<TwitterUI_Tweets.TweetView> tweetList = new List<TwitterUI_Tweets.TweetView>();

                // Get the user
                IUser user = twitter.GetUserByScreenName("@" + userLoaded);

                if (user == null)
                    return false;

                IRelationshipDetails rd = twitter.GetFriendShipWith(user.Id);

                string name = user.Name;
                int followersNum = user.FollowersCount;
                int followingNum = user.FriendsCount;
                string description = user.Description;

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                {
                    InfoAboutBox.Header = "Acerca de " + user.ScreenName;
                    UserName.Content = name;
                    UserDescription.Text = description;
                    NumFollowers.Content = followersNum;
                    NumFollowing.Content = followingNum;

                    var link = new Hyperlink(new Run("@" + user.ScreenName))
                    {
                        NavigateUri = new Uri("https://www.twitter.com/" + user.ScreenName)
                    };

                    // Open the account uri on an externar browser
                    link.RequestNavigate += (o, e) =>
                    {
                        e.Handled = true;
                        Process.Start(e.Uri.ToString());
                    };

                    LinkToProfile.Content = link;

                    if (rd.Following)
                    {
                        Unfollow.IsEnabled = true;
                        Unfollow.Click += (s, e) => twitter.Unfollow(user);
                        Follow.IsEnabled = false;
                    }
                    else
                    {
                        Follow.IsEnabled = true;
                        Follow.Click += (s, e) => twitter.Follow(user);
                        Unfollow.IsEnabled = false;
                    }
                }));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Enable and disable buttons
        private void OwnTimeLineContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(TweetsContainer.SelectedItem == null)
            {
                FavButton.IsEnabled = false;
                RetButton.IsEnabled = false;
            }
            else
            {
                FavButton.IsEnabled = true;
                RetButton.IsEnabled = true;
            }
        }

        private void FavButton1_Click(object sender, RoutedEventArgs e)
        {
            TwitterUI_Tweets.FavoriteTweet(twitter, ((TwitterUI_Tweets.TweetView)TweetsContainer.SelectedItem).Tweet);
        }

        private void RetButton1_Click(object sender, RoutedEventArgs e)
        {
            TwitterUI_Tweets.Retweet(twitter, ((TwitterUI_Tweets.TweetView)TweetsContainer.SelectedItem).Tweet);
        }

        private void UsersSuggestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(UsersSuggestion.SelectedItem != null)
            {
                UserSearch.Text = UsersSuggestion.SelectedItem as string;
            }
        }
    }
}
