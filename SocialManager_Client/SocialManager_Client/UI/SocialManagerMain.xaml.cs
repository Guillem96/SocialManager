using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for SocialManagerMain.xaml
    /// </summary>
    public partial class SocialManagerMain : Window
    {
        private Timer checkFriends;

        public SocialManagerMain()
        {
            InitializeComponent();
            logoImage.Source = PathUtilities.GetImageSource("Logo.png");
            LogoutImage.Source = PathUtilities.GetImageSource("Logout.png");
            EditProfileImage.Source= PathUtilities.GetImageSource("EditProfile.png");
            MainWindow.Title = "Social Manager - " + ClientController.client.Profile.Username;

            // Set timer to check which contacts are online
            checkFriends = new Timer()
            {
                Enabled = true,
                Interval = 3000 // Check every 3 seconds
            };
            checkFriends.Elapsed += (s, e) => CheckFriends();
        }

        private void ContactsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ContactsUI());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            string message = "";
            if (!ClientController.client.Logout(out message))
                MessageBox.Show(message);
            else
            {
                this.Close();
                new LoginWindow().ShowDialog();
            }
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            new EditProfileWindow().ShowDialog();
        }

        private void CheckFriends()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Fill the list view
                ConnectedFriends.Items.Clear();
                foreach (var f in ClientController.client.Profile.Contacts)
                {
                    StackPanel sp = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5, 5, 5, 5)
                    };

                    sp.Children.Add(new Image()
                    {
                        Source = PathUtilities.GetImageSource("Profile.png"),
                        Width = 30,
                        Height = 30,
                        Margin = new Thickness(5, 2, 5, 0)
                    });

                    sp.Children.Add(new Label()
                    {
                        Content = f.Profile.Username,
                        Margin = new Thickness(5, 2, 5, 0)
                    });

                    Button b = new Button()
                    {
                        Content = new Image() { Source = PathUtilities.GetImageSource("newChat.png") },
                        Width = 25,
                        Height = 25,
                        Background = Brushes.Transparent,
                        BorderBrush = Brushes.Transparent,
                        Margin = new Thickness(5, 2, 5, 0)
                    };
                    b.Click += (o, e) => AddNewChat(f);

                    sp.Children.Add(b);

                    sp.Children.Add(new Image()
                    {
                        Source = PathUtilities.GetImageSource((f.Stat == Contact.Status.Disconnected)
                                                                ? "disconnected.png"
                                                                : "connected.png"),
                        Width = 10,
                        Height = 10,
                        Margin = new Thickness(5, 2, 5, 0)
                    });

                    ConnectedFriends.Items.Add(sp);
                }
            }));
        }

        private void AddNewChat(Contact to)
        {
            Expander e = new Expander()
            {
                Header = to.Profile.Username,
                ExpandDirection = ExpandDirection.Up,
                Width = 285,
                Margin = new Thickness(-0,-350,0, 0),
                Content = new Frame()
                {
                    Width = 280,
                    Height = 350,
                    NavigationUIVisibility = NavigationUIVisibility.Hidden
                }
            };
           
            ((Frame)e.Content).Navigate(new Chat(to));

            ChatsStack.Children.Add(e);
        }
    }
}
