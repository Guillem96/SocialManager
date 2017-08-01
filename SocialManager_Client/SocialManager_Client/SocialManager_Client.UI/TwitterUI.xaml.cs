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
            Thread t = new Thread(() => LoadData()) { IsBackground = true };
            t.Start();
            
        }

        
        public void LoadData()
        {

            Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingGridHeight, loadingGridWidth, LoadingGrid, "Twitter login...")));

            
                if (twitter == null)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MainGrid.Children.Clear();
                        MainGrid.Children.Add(new TextBlock()
                        {
                            Text = "Configura tu twitter en la opción Configurar Twitter/Instagram para usar esta función. Gracias.",
                            Width = 250,
                            FontSize = 20,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            TextWrapping = TextWrapping.Wrap
                        });
                    }));
                }
                else
                {
                this.twitter.Login();
                Dispatcher.BeginInvoke(new Action(() => contentFrame.Navigate(new TwitterUI_Tweets(twitter))));
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

        private void SearchUsersButton_Click(object sender, RoutedEventArgs e)
        {
            UserControl uc = new TwitterUI_Search(twitter);
            contentFrame.Navigate(uc);
        }
    }
}
