
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Tweetinvi.Models;

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for TwitterUI_Tweets.xaml
    /// </summary>
    public partial class TwitterUI_Tweets : UserControl
    {
        public class TweetView
        {
            public ITweet Tweet { get; set; }
            public object UserImage { get; set; }
            public string TweetText { get; set; }
        }

        private SocialNetworksLogic.Twitter twitter;

        private double gridHeight;
        private double gridWidth;

        private OpenFileDialog ofd;

        private string mediaPath = "";

        internal TwitterUI_Tweets(SocialNetworksLogic.Twitter twitter)
        {
            InitializeComponent();

            // Set up open file dialog
            ofd = new OpenFileDialog()
            {
                Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif"
            };
            ofd.FileOk += (o, e) => SetMediaPath();

            this.twitter = twitter;

            // Set up the buttons
            FavButton1.IsEnabled = false;
            FavButton2.IsEnabled = false;
            RetButton2.IsEnabled = false;
            RetButton1.IsEnabled = false;
            DeleteTweetButton.IsEnabled = false;

            FavButton1.Content = new Image() { Source = PathUtilities.GetImageSource("heart.png") };
            FavButton2.Content = new Image() { Source = PathUtilities.GetImageSource("heart.png") };
            RetButton2.Content = new Image() { Source = PathUtilities.GetImageSource("retweet.png") };
            RetButton1.Content = new Image() { Source = PathUtilities.GetImageSource("retweet.png") };
            DeleteTweetButton.Content = new Image() { Source = PathUtilities.GetImageSource("delete.png") };

            AddImageButton.Content = new Image() { Source = PathUtilities.GetImageSource("addImage.png") };
            HomeTweets.Source = PathUtilities.GetImageSource("home.png");
            UserTweets.Source = PathUtilities.GetImageSourceFromUri(twitter.GetProfileImage());

            RefreshButton.Content = new Image() { Source = PathUtilities.GetImageSource("refresh.png") };

            // Grid size
            gridHeight = LoadingGrid1.Height;
            gridWidth = LoadingGrid1.Width;

            // Fill list views
            new Thread(new ThreadStart(() => LoadTweets())) { IsBackground = true }.Start();

            // Banner background
            new Thread(new ThreadStart(() => LoadBannerImage())) { IsBackground = true }.Start();

        }

        private void LoadTweets()
        {
            // Start loading images
            Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(gridHeight, gridWidth, LoadingGrid1, "Cargando tweets",70,70)));
            Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(gridHeight, gridWidth, LoadingGrid2, "Cargando tweets", 70, 70)));

            // Fill both listviews
            // If filled correctly end loading, else continue loading until the next refresh
            new Thread(new ThreadStart(() => 
            {
                if (FillTweets(OwnTimeLineContainer, true))
                    Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGrid1)));
            }))
            { IsBackground = true }.Start();

            new Thread(new ThreadStart(() =>
            {
                if (FillTweets(HomeTweetsContainer, false))
                    Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGrid2)));
            }))
            { IsBackground = true }.Start();
        }

        // Fill the containers
        private bool FillTweets(ListView Container, bool own)
        {
            try
            {
                List<TweetView> tweetList = new List<TweetView>();

                List<ITweet> tweets = null;
                List<string> urls = new List<string>();

                if (own)
                    tweets = twitter.GetProfileTweets(10).ToList();
                else
                    tweets = twitter.GetHomeTweets(15).ToList();

                // Get the urls
                foreach (var t in tweets)
                {
                    urls.Add(twitter.GetProfileImage(t.CreatedBy.Id));
                }

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {

                    int SelectedIndex = Container.SelectedIndex;
                    for (int i = 0; i < tweets.Count; i++)
                    {
                        tweetList.Add(new TweetView()
                        {
                            Tweet = tweets[i],
                            TweetText = tweets[i].Text,
                            UserImage = PathUtilities.GetImageSourceFromUri(urls[i])
                        });
                    }
                    Container.ItemsSource = tweetList;
                    Container.SelectedIndex = SelectedIndex;
                }));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void LoadBannerImage()
        {
            string bannerUrl = twitter.GetBannerImageUrl();

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() => {
                MainGrid.Background = new ImageBrush()
                {
                    ImageSource = bannerUrl == null ? null : PathUtilities.GetImageSourceFromUri(bannerUrl),
                    Stretch = Stretch.UniformToFill,
                    Opacity = 0.2
                };
            })); 
        }
        // Enable or disable the buttons depending on the selected item
        private void HomeTweetsContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(HomeTweetsContainer.SelectedItem == null && OwnTimeLineContainer.SelectedItem == null)
            {
                IsReply.IsEnabled = false;
            }
            else if(HomeTweetsContainer.SelectedItem != null)
            {
                // Only one tweet can be selcted
                OwnTimeLineContainer.SelectedIndex = -1;
                OwnTimeLineContainer.SelectedItem = null;
                IsReply.IsEnabled = true;
                FavButton2.IsEnabled = true;
                RetButton2.IsEnabled = true;
                return;
            }
            
            FavButton2.IsEnabled = false;
            RetButton2.IsEnabled = false;
            
        }

        // Enable or disable the buttons depending on the selected item
        private void OwnTimeLineContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OwnTimeLineContainer.SelectedItem == null && HomeTweetsContainer.SelectedItem == null)
            {
                IsReply.IsEnabled = false;
            }
            else if(OwnTimeLineContainer.SelectedItem != null)
            {
                // Only one tweet can be selcted
                HomeTweetsContainer.SelectedIndex = -1;
                HomeTweetsContainer.SelectedItem = null;
                IsReply.IsEnabled = true;
                FavButton1.IsEnabled = true;
                RetButton1.IsEnabled = true;
                DeleteTweetButton.IsEnabled = true;
                return;
            }
            FavButton1.IsEnabled = false;
            RetButton1.IsEnabled = false;
            DeleteTweetButton.IsEnabled = false;

        }

        // Get a label from an uri
        private Label LabelFromUri(string text, string uri)
        {
            // Link to account
            var link = new Hyperlink(new Run(text))
            {
                NavigateUri = new Uri(uri),
            };

            // Open the account uri on an externar browser
            link.RequestNavigate += (o, e) =>
            {
                e.Handled = true;
                Process.Start(e.Uri.ToString());
            };

            return new Label()
            {
                Content = link,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        // Twitter user actions
        internal static void FavoriteTweet(SocialNetworksLogic.Twitter twitter, ITweet t)
        {
            twitter.FavoriteTweet(t.Id);
            MessageBox.Show("Has añadido el tweet de " + t.CreatedBy.ScreenName + " a favoritos.");
        }

        internal static void Retweet(SocialNetworksLogic.Twitter twitter, ITweet t)
        {
            twitter.Retweet(t.Id);
            MessageBox.Show("Has retweeteado el tweet de " + t.CreatedBy.ScreenName + ".");
        }

        private void PublishTweetButton_Click(object sender, RoutedEventArgs e)
        {
            if (TweetBody.Text == "") return;

            if (IsReply.IsChecked.Value)
            {
                var tweetToReply = HomeTweetsContainer.SelectedItem == null 
                                        ? ((TweetView)OwnTimeLineContainer.SelectedItem).Tweet 
                                        : ((TweetView)HomeTweetsContainer.SelectedItem).Tweet;

                if(twitter.ReplyTweet(tweetToReply.Id, TweetBody.Text) == null)
                {
                    // Error
                    MessageBox.Show("Error al publicar el tweet.");
                    return;
                }

                MessageBox.Show("Has respondido el tweet de " + tweetToReply.CreatedBy.ScreenName + " correctamente.");
            }
            else
            {
                if(twitter.PublishTweet(TweetBody.Text, mediaPath) == null)
                {
                    // Error
                    MessageBox.Show("Error al publicar el tweet.");
                    return;
                }

                // Reset media path for the next tweet
                mediaPath = "";
                ImageName.Content = "";

                MessageBox.Show("Has tweeteado el tweet \"" + TweetBody.Text + "\" correctamente.");
            }

            TweetBody.Text = "En que estás pensando...";
        }

        // Following 4 functions fav or retweet any selected tweet
        private void RetButton2_Click(object sender, RoutedEventArgs e)
        {
            Retweet(twitter, ((TweetView)HomeTweetsContainer.SelectedItem).Tweet);
        }

        private void FavButton2_Click(object sender, RoutedEventArgs e)
        {
            FavoriteTweet(twitter, ((TweetView)HomeTweetsContainer.SelectedItem).Tweet);
        }

        private void FavButton1_Click(object sender, RoutedEventArgs e)
        {
            FavoriteTweet(twitter, ((TweetView)OwnTimeLineContainer.SelectedItem).Tweet);
        }

        private void RetButton1_Click(object sender, RoutedEventArgs e)
        {
            Retweet(twitter, ((TweetView)OwnTimeLineContainer.SelectedItem).Tweet);
        }

        // Reload the tweets
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            new Thread(new ThreadStart(() => LoadTweets())) { IsBackground = true }.Start();
        }

        // Add an image to your tweet
        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            ofd.ShowDialog();
        }

        private void SetMediaPath()
        {
            mediaPath = ofd.FileName;
            FileInfo fI = new FileInfo(mediaPath);
            ImageName.Content = fI.Name;
        }

        private void DeleteTweetButton_Click(object sender, RoutedEventArgs e)
        {
            twitter.DeleteTweet(((TweetView)OwnTimeLineContainer.SelectedItem).Tweet.Id);
            MessageBox.Show("Tweet eliminado correctamente.");
            new Thread(new ThreadStart(() => LoadTweets())) { IsBackground = true }.Start();
        }
    }
}
