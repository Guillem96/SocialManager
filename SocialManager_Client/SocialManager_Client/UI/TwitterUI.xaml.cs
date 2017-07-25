using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for TwitterUI.xaml
    /// </summary>
    public partial class TwitterUI : UserControl
    {
        private SocialNetworksLogic.Twitter twitter;
        private UserControl openedFrame;

        internal TwitterUI(SocialNetworksLogic.Twitter twitter)
        {
            InitializeComponent();

            TwitterLogo.Source = PathUtilities.GetImageSource("twitter.png");
            TwitterText.Source = PathUtilities.GetImageSource("twittertext.png");

            if(twitter == null)
            {
                // TODO: Don't show UI -> Empty main greed
                MainGrid.Children.Clear();
            }
            else
            {
                this.twitter = twitter;
                this.twitter.Login();
                
            }
        }

        private void SetOpenFrame(UserControl newUserControl)
        {
            if (openedFrame == null)
            {
                openedFrame = newUserControl;
            }
            else
            {
                // Dispose the frame and variables
                switch (openedFrame.GetType().ToString())
                {
                    case "SocialManager_Client.UI.TwitterUI_Tweets":
                        break;
                   
                }
                openedFrame = newUserControl;
            }
        }

        private void TweetsButton_Click(object sender, RoutedEventArgs e)
        {
            UserControl uc = new TwitterUI_Tweets(twitter);
            SetOpenFrame(uc);
            contentFrame.Navigate(uc);
        }
    }
}
