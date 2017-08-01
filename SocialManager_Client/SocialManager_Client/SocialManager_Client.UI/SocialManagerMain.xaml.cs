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
using System.Threading;

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for SocialManagerMain.xaml
    /// </summary>
    public partial class SocialManagerMain : Window
    {
        private System.Timers.Timer checkFriends;
        private Dictionary<string, Expander> instantiatedChats; //< Reference to chats, <username, chat>
        private UserControl openedFrame; //< Reference to opened user control

        public SocialManagerMain()
        {
            InitializeComponent();

            // Set images
            logoImage.Source = PathUtilities.GetImageSource("Logo.png");
            LogoutImage.Source = PathUtilities.GetImageSource("Logout.png");
            EditProfileImage.Source= PathUtilities.GetImageSource("EditProfile.png");
            MainWindow.Title = "Social Manager - " + ClientController.client.Profile.Username;

            // References to instanciated chats
            instantiatedChats = new Dictionary<string, Expander>();

            // Set timer to check which contacts are online
            checkFriends = new System.Timers.Timer()
            {
                Enabled = true,
                Interval = 3000 // Check every 3 seconds
            };
            checkFriends.Elapsed += (s, e) => CheckFriends();
        }

        private void SetOpenFrame(UserControl newUserControl)
        {
            if(openedFrame == null)
            {
                openedFrame = newUserControl;
            }
            else
            {
                // Dispose the frame and variables
                switch (openedFrame.GetType().ToString())
                {
                    case "SocialManager_Client.UI.ContactsUI":
                        ((ContactsUI)openedFrame).Close();
                        break;
                    case "SocialManager_Client.UI.AgendaUI":
                        ((AgendaUI)openedFrame).Close();
                        break;
                }
                openedFrame = newUserControl;
            }
        }

        // Right navbar
        private void ContactsButton_Click(object sender, RoutedEventArgs e)
        {
            UserControl nUserControl = new ContactsUI();
            SetOpenFrame(nUserControl);
            ContentFrame.Navigate(nUserControl);
        }

        private void AgendaButton_Click(object sender, RoutedEventArgs e)
        {
            UserControl nUserControl = new AgendaUI();
            SetOpenFrame(nUserControl);
            ContentFrame.Navigate(nUserControl);
        }

        private void SNSetUp_Click(object sender, RoutedEventArgs e)
        {
            UserControl nUserControl = new SetUpSocialNetworkUI();
            SetOpenFrame(nUserControl);
            ContentFrame.Navigate(nUserControl);
        }

        private void TwitterButton_Click(object sender, RoutedEventArgs e)
        {
            ClientController.client.TwitterLogin();
            UserControl nUserControl = new TwitterUI(ClientController.client.Twitter);
            SetOpenFrame(nUserControl);
            ContentFrame.Navigate(nUserControl);
        }

        private void Instagram_Click(object sender, RoutedEventArgs e)
        {
            ClientController.client.InstagramLogin();
            UserControl nUserControl = new InstagramUI(ClientController.client.Instagram);
            SetOpenFrame(nUserControl);
            ContentFrame.Navigate(nUserControl);
        }

        // Upper right buttons actions
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ClientController.client.Logout(out string message))
                MessageBox.Show(message);
            else
            {
                SetOpenFrame(null);
                this.Close();
                new LoginWindow().ShowDialog();
            }
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            new EditProfileWindow().ShowDialog();
        }


        // Messages related functions
        private void CheckFriends()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Fill the list view
                ConnectedFriends.Items.Clear();

                bool newMessages = false;

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

                    if (!newMessages)
                        newMessages = ClientController.AnyMessageFrom(f.Profile.Username);

                    int width;
                    int height;
                    string imageSource;

                    if (ClientController.AnyMessageFrom(f.Profile.Username))
                    {
                        width = 35;
                        height = 35;
                        imageSource = "newChatNewMessages.png";
                    }
                    else
                    {
                        width = 25;
                        height = 25;
                        imageSource = "newChat.png";
                    }

                    Button b = new Button()
                    {
                        Content = new Image()
                        {
                            Source = PathUtilities.GetImageSource(imageSource),
                        },
                        Width = width,
                        Height = height,
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
                if (newMessages)
                    NewMessagesImage.Source = PathUtilities.GetImageSource("new.png");
                else
                    NewMessagesImage.Source = null;

            }));
        }

        private void AddNewChat(Contact to)
        {
            if(instantiatedChats.Count == 3)
            {
                MessageBox.Show("No puedes abrir más de 3 chats.");
                return;
            }

            if (instantiatedChats.Keys.Contains(to.Profile.Username))
            {
                instantiatedChats[to.Profile.Username].IsExpanded = true;
                return;
            }

            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };

            Button b = new Button()
            {
                Content = new Image() { Source = PathUtilities.GetImageSource("deny.png") },
                Width = 15,
                Height = 15,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent
            };

            sp.Children.Add(new Label() { Content = to.Profile.Username, Margin = new Thickness(5,0,5,0) });
            sp.Children.Add(b);

            Expander e = new Expander()
            {
                Header = sp,
                ExpandDirection = ExpandDirection.Up,
                Width = 205,
                Margin = new Thickness(-0, -350, 0, 0),
                IsExpanded = true,
                Content = new Frame()
                {
                    Width = 200,
                    Height = 350,
                    NavigationUIVisibility = NavigationUIVisibility.Hidden
                }
            };
           
            ((Frame)e.Content).Navigate(new Chat(to));

            ChatsStack.Children.Add(e);

            b.Click += (o, s) =>
                                {
                                    ChatsStack.Children.Remove(e);
                                    instantiatedChats.Remove(to.Profile.Username);
                                };

            instantiatedChats.Add(to.Profile.Username, e);
        }
        
    }
}
