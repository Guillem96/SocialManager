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
using InstagramGot.Models;
using System.Threading;
using System.Diagnostics;

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for InstagramUI_Users.xaml
    /// </summary>
    public partial class InstagramUI_Users : UserControl
    {
        private enum UserType { Follower, Following, Requested }

        public class UserView
        {
            public IMinifiedUser User { get; set; }
            public object UserImage { get => PathUtilities.GetImageSourceFromUri(User.ProfileImageUrl); set => UserImage = value; }
            public string UserName { get => User.Username; set => User.Username = value; }
            public string UserUri { get => "https://instagram.com/" + UserName; set => UserUri = value; }
        }

        private SocialNetworksLogic.Instagram instagram;

        private double loadingGridHeight;
        private double loadingGridWidth;

        internal InstagramUI_Users(SocialNetworksLogic.Instagram instagram)
        {
            InitializeComponent();

            this.instagram = instagram;

            // Set up buttons
            Refresh.Content = new Image() { Source = PathUtilities.GetImageSource("refresh.png") };
            FollowFollower.Content = new Image() { Source = PathUtilities.GetImageSource("add.png") };
            UnFollowFollowing.Content = new Image() { Source = PathUtilities.GetImageSource("delete.png") };
            DeclineButton.Content = new Image() { Source = PathUtilities.GetImageSource("delete.png") };
            ApproveButton.Content = new Image() { Source = PathUtilities.GetImageSource("add.png") };

            FollowFollower.IsEnabled = false;
            DeclineButton.IsEnabled = false;
            ApproveButton.IsEnabled = false;
            UnFollowFollowing.IsEnabled = false;

            // Set grid size
            loadingGridHeight = LoadingGridFollowers.Height;
            loadingGridWidth = LoadingGridFollowers.Width;

            // Fill the containers
            new Thread(new ThreadStart(() => LoadData())).Start();

        }

        void LoadData()
        {
            Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingGridHeight, loadingGridWidth, LoadingGridFollowers, "Cargando Seguidores...", 70, 70)));
            Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingGridHeight, loadingGridWidth, LoadingGridFriends, "Cargando Seguidos...", 70, 70)));
            Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingGridHeight, loadingGridWidth, LoadingGridTraitors, "Cargando Traidores...", 70, 70)));

            new Thread(new ThreadStart(() =>
            {
                if (FillUserContainers(UserType.Follower))
                    Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGridFollowers)));
            })).Start();

            new Thread(new ThreadStart(() =>
            {
                if (FillUserContainers(UserType.Following))
                    Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGridFriends)));
            })).Start();

            new Thread(new ThreadStart(() =>
            {
                if (FillUserContainers(UserType.Requested))
                    Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGridTraitors)));
            })).Start();

            new Thread(new ThreadStart(() => SetRandomBackground())).Start();

        }

        private void SetRandomBackground()
        {
            // Get a random media
            Random gen = new Random();

            // Get Media from user
            var media = instagram.GetRecentMedia(20);

            var backgroundImage = media[gen.Next(0, media.Count)].ImageStandardResolutionUrl;

            // Set it as background
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                MainGrid.Background = new ImageBrush(PathUtilities.GetImageSourceFromUri(backgroundImage))
                {
                    Opacity = 0.3,
                    Stretch = Stretch.UniformToFill
                };
            }));
        }

        bool FillUserContainers(UserType type)
        {
            try
            {
                List<UserView> userList = new List<UserView>();

                var users = new List<IMinifiedUser>();

                List<string> urls = new List<string>();

                ListView Container = null;

                switch (type)
                {
                    case UserType.Follower:
                        Container = FollowersContainer;
                        users = instagram.GetFollowedBy();
                        break;
                    case UserType.Following:
                        Container = FriendsContainer;
                        users = instagram.GetFollows();
                        break;
                    case UserType.Requested:
                        users = instagram.GetRequestedBy();
                        Container = TraitorsContainer;
                        break;
                    default:
                        return false;
                }

                // Get the urls
                foreach (var t in users)
                {
                    urls.Add(instagram.GetUserById(t.Id).ProfilePictureUrl);
                }

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    switch (type)
                    {
                        case UserType.Follower:
                            FollowersNumber.Content = users.Count.ToString();
                            break;
                        case UserType.Following:
                            FollowingNumber.Content = users.Count.ToString();
                            break;
                        case UserType.Requested:
                            TraitorsNumber.Content = users.Count.ToString();
                            break;
                    }

                    int SelectedIndex = Container.SelectedIndex;
                    for (int i = 0; i < users.Count; i++)
                    {
                        userList.Add(new UserView()
                        {
                            User = users[i],
                            UserImage = PathUtilities.GetImageSourceFromUri(urls[i]),
                        });
                    }

                    Container.ItemsSource = userList;
                    Container.SelectedIndex = SelectedIndex;
                }));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Handle hyperlink event to open on web browser
        private void HandleLinkClick(object sender, RoutedEventArgs e)
        {

            Hyperlink hl = (Hyperlink)sender;

            string navigateUri = hl.NavigateUri.ToString();

            Process.Start(new ProcessStartInfo(navigateUri));

            e.Handled = true;

        }

        // Enable and disable buttons depending the selected item
        private void FollowersContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FollowersContainer.SelectedItem == null)
            {
                FollowFollower.IsEnabled = false;
            }
            else
            {
                FollowFollower.IsEnabled = true;
            }
        }

        private void FriendsContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FriendsContainer.SelectedItem == null)
            {
                UnFollowFollowing.IsEnabled = false;
            }
            else
            {
                UnFollowFollowing.IsEnabled = true;
            }
        }

        private void TraitorsContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TraitorsContainer.SelectedItem == null)
            {
                DeclineButton.IsEnabled = false;
                ApproveButton.IsEnabled = false;

            }
            else
            {
                DeclineButton.IsEnabled = true;
                ApproveButton.IsEnabled = true;

            }
        }

        // User actions
        internal static void Follow(IMinifiedUser user, SocialNetworksLogic.Instagram instagram)
        {
            if (!instagram.Follow(user.Id))
            {
                MessageBox.Show("No puedes seguir a " + user.Username);
            }
            else
            {
                MessageBox.Show("Has empezado a seguir a " + user.Username);
            }
        }

        internal static void Unfollow(IMinifiedUser user, SocialNetworksLogic.Instagram instagram)
        {
            if (!instagram.Unfollow(user.Id))
            {
                MessageBox.Show("No puedes dejar de seguir a " + user.Username);
            }
            else
            {
                MessageBox.Show("Has dejado de seguir a " + user.Username);
            }
        }

        internal static void Approve(IMinifiedUser user, SocialNetworksLogic.Instagram instagram)
        {
            if (!instagram.Approve(user.Id))
            {
                MessageBox.Show("No puedes seguir a " + user.Username);
            }
            else
            {
                MessageBox.Show("Has empezado a seguir a " + user.Username);
            }
        }

        internal static void Ignore(IMinifiedUser user, SocialNetworksLogic.Instagram instagram)
        {
            if (!instagram.Ignore(user.Id))
            {
                MessageBox.Show("No puedes dejar de seguir a " + user.Username);
            }
            else
            {
                MessageBox.Show("Has dejado de seguir a " + user.Username);
            }
        }

        private void FollowFollower_Click(object sender, RoutedEventArgs e)
        {
            Follow(((UserView)FollowersContainer.SelectedItem).User, instagram);
        }

        private void UnFollowFollowing_Click(object sender, RoutedEventArgs e)
        {
            Unfollow(((UserView)FriendsContainer.SelectedItem).User, instagram);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            FollowFollower.IsEnabled = false;
            DeclineButton.IsEnabled = false;
            ApproveButton.IsEnabled = false;
            UnFollowFollowing.IsEnabled = false;

            // Fill the containers
            new Thread(new ThreadStart(() => LoadData())).Start();
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            Approve((TraitorsContainer.SelectedItem as UserView).User, instagram);
        }

        private void DeclineButton_Click(object sender, RoutedEventArgs e)
        {
            Ignore((TraitorsContainer.SelectedItem as UserView).User, instagram);
        }
    }
}
