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

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for InstagramUI.xaml
    /// </summary>
    public partial class InstagramUI : UserControl
    {
        private SocialNetworksLogic.Instagram instagram;

        // Grid size
        private double loadingGridHeight;
        private double loadingGridWidth;

        internal InstagramUI(SocialNetworksLogic.Instagram instagram)
        {
            InitializeComponent();

            // Get reference to instagram
            this.instagram = instagram;

            // Set loading grid size
            loadingGridHeight = LoadingGrid.Height;
            loadingGridWidth = LoadingGrid.Width;

            // Set up images
            InstagramHeader.Source = PathUtilities.GetImageSource("InstagramHeader.png");

            // Load instagram
            Thread t = new Thread(() => LoadData()) { IsBackground = true };
            t.Start();
        }

        // Load instagram data
        public void LoadData()
        {
            // init loading
            Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingGridHeight, loadingGridWidth, LoadingGrid, "Instagram login...")));

            // If instagram is not set up
            if (instagram == null)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(new TextBlock()
                    {
                        Text = "Configura tu instagram en la opción Configurar Twitter/Instagram para usar esta función. Gracias.",
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
                // Else, if instagram is set up then do login
                instagram.Login();
                Dispatcher.BeginInvoke(new Action(() => contentFrame.Navigate(new InstagramUI_Media(instagram))));
            }
            // End loading screen
            Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGrid)));
        }

        // Navigate functions
        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(new InstagramUI_Users(instagram));
        }

        private void MediaButton_Click(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(new InstagramUI_Media(instagram));
        }

        private void SearchUsersButton_Click(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(new InstagramUI_Search(instagram));
        }
    }
}
