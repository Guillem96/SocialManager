
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Tweetinvi.Models;

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for TwitterUI_Tweets.xaml
    /// </summary>
    public partial class TwitterUI_Tweets : UserControl
    {
        private SocialNetworksLogic.Twitter twitter;

        internal TwitterUI_Tweets(SocialNetworksLogic.Twitter twitter)
        {
            InitializeComponent();
            this.twitter = twitter;

            FillTweets(OwnTimeLineContainer, twitter.GetProfileTweets(10).ToList());
        }

        private void FillTweets(ListView Container, List<ITweet> tweets)
        {
            Dispatcher.Invoke(new Action(() =>
            {

                Container.Items.Clear();

                foreach (ITweet tweet in tweets)
                {
                    StackPanel sp = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };

                    Image i = new Image()
                    {
                        Width = 40,
                        Height = 40,
                        Source = PathUtilities.GetImageSourceFromUri(twitter.GetProfileImage(tweet.CreatedBy.Id)),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    sp.Children.Add(i);

                    sp.Children.Add(new TextBox()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        BorderBrush = Brushes.Transparent,
                        Width = 130,
                        IsReadOnly = true,
                        TextWrapping = TextWrapping.Wrap,
                        Text = tweet.Text,
                    });

                    Button fav = new Button()
                    {
                        Width = 20,
                        Height = 20,
                        Content = new Image() { Source = PathUtilities.GetImageSource("heart.png"), Stretch = Stretch.Fill },
                        VerticalAlignment = VerticalAlignment.Center,
                        Background = Brushes.Transparent,
                        BorderBrush = Brushes.Transparent
                    };

                    Button ret = new Button()
                    {
                        Width = 20,
                        Height = 20,
                        Content = new Image() { Source = PathUtilities.GetImageSource("retweet.png"), Stretch = Stretch.Fill },
                        VerticalAlignment = VerticalAlignment.Center,
                        Background = Brushes.Transparent,
                        BorderBrush = Brushes.Transparent
                    };

                    sp.Children.Add(fav);
                    sp.Children.Add(ret);
                    Container.Items.Add(sp);
                }
            }),System.Windows.Threading.DispatcherPriority.Send);
        }

        private void IsReply_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void PublishTweetButton_Click(object sender, RoutedEventArgs e)
        {

        }

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

            }
        }

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

            }
        }

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
    }
}
