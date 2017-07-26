using System;
using System.Collections.Generic;
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
using WpfAnimatedGif;

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for TwitterUI.xaml
    /// </summary>
    public partial class TwitterUI : UserControl
    {
        private SocialNetworksLogic.Twitter twitter;
        private UserControl openedFrame = null;

        private double loadingGridHeight;
        private double loadingGridWidth;

        internal TwitterUI(SocialNetworksLogic.Twitter twitter)
        {
            InitializeComponent();

            // Reference to twitter
            this.twitter = twitter;

            // Set loading grid size
            loadingGridHeight = LoadingGrid.Height;
            loadingGridWidth = LoadingGrid.Width;

            TwitterLogo.Source = PathUtilities.GetImageSource("twitter.png");
            TwitterText.Source = PathUtilities.GetImageSource("twittertext.png");

            // Load twitter
            new Thread(() => LoadData()).Start();
        }

        
        public void LoadData()
        {

            Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingGridHeight, loadingGridWidth, LoadingGrid, "Twitter login...")));

            if (twitter == null)
            {
                MainGrid.Children.Clear();
            }
            else
            {
                this.twitter.Login();
            }
            Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGrid)));
        }


        private void TweetsButton_Click(object sender, RoutedEventArgs e)
        {
            UserControl uc = new TwitterUI_Tweets(twitter);
            contentFrame.Navigate(uc);
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            UserControl uc = new TwitterUI_Users(twitter);
            contentFrame.Navigate(uc);
        }
    }
}
