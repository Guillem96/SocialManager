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
    /// Interaction logic for TwitterUI_Users.xaml
    /// </summary>
    public partial class TwitterUI_Users : UserControl
    {
        private enum UserType { Follower, Following, Traitors }

        public class UserView
        {
            public IUser User { get; set; }
            public object UserImage { get; set; }
            public string UserName { get => User.ScreenName; set => User.ScreenName = value; }
            public string UserUri { get => "https://twitter.com/" + UserName; set => UserUri = value; }
        }

        private SocialNetworksLogic.Twitter twitter;

        private double loadingGridHeight;
        private double loadingGridWidth;

        internal TwitterUI_Users(SocialNetworksLogic.Twitter twitter)
        {
            InitializeComponent();

            this.twitter = twitter;

            // Set up buttons
            Refresh.Content = new Image() { Source = PathUtilities.GetImageSource("refresh.png") };
            FollowFollower.Content = new Image() { Source = PathUtilities.GetImageSource("add.png") };
            UnFollowFollowing.Content = new Image() { Source = PathUtilities.GetImageSource("delete.png") };
            UnFollowTraitor.Content = new Image() { Source = PathUtilities.GetImageSource("delete.png") };

            FollowFollower.IsEnabled = false;
            UnFollowTraitor.IsEnabled = false;
            UnFollowFollowing.IsEnabled = false;

            // Set grid size
            loadingGridHeight = LoadingGridFollowers.Height;
            loadingGridWidth = LoadingGridFollowers.Width;

            // Fill the containers
            new Thread(new ThreadStart(() => LoadData())).Start();

            new Thread(new ThreadStart(() => LoadBannerImage())).Start();
        }

        void LoadData()
        {
            Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingGridHeight, loadingGridWidth, LoadingGridFollowers, "Cargando Seguidores...",70,70)));
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
                if (FillUserContainers(UserType.Traitors))
                    Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGridTraitors)));
            })).Start();

        }

        void LoadBannerImage()
        {
            string bannerUrl = twitter.GetBannerImageUrl();

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            {
                MainGrid.Background = new ImageBrush()
                {
                    ImageSource = bannerUrl == null ? null : PathUtilities.GetImageSourceFromUri(bannerUrl),
                    Stretch = Stretch.UniformToFill,
                    Opacity = 0.2
                };
            }));
            
        }

        bool FillUserContainers(UserType type)
        {
            try
            {
                List<UserView> userList = new List<UserView>();

                List<IUser> users = null;
                List<string> urls = new List<string>();

                ListView Container = null;

                switch (type)
                {
                    case UserType.Follower:
                        Container = FollowersContainer;
                        users = twitter.GetFollowers().ToList();
                        break;
                    case UserType.Following:
                        Container = FriendsContainer;
                        users = twitter.GetFriends().ToList();
                        break;
                    case UserType.Traitors:
                        var followers = twitter.GetFollowers().Select(f => f.Id).ToList();
                        var following = twitter.GetFriends().Select(f => f.Id).ToList();
                        users = following
                                    .Except(followers)
                                    .Select(f => twitter.GetUserById(f))
                                    .ToList();

                        Container = TraitorsContainer;
                        break;
                    default:
                        return false;
                }

                // Get the urls
                foreach (var t in users)
                {
                    urls.Add(twitter.GetProfileImage(t.Id));
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
                        case UserType.Traitors:
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
            if(FollowersContainer.SelectedItem == null)
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
                UnFollowTraitor.IsEnabled = false;
            }
            else
            {
                UnFollowTraitor.IsEnabled = true;
            }
        }

        // User actions
        private void Follow(IUser user)
        {
            if (!twitter.Follow(user))
            {
                MessageBox.Show("No puedes seguir a " + user.ScreenName);
            }
            else
            {
                MessageBox.Show("Has empezado a seguir a " + user.ScreenName);
            }
        }

        private void Unfollow(IUser user)
        {
            if (!twitter.Unfollow(user))
            {
                MessageBox.Show("No puedes dejar de seguir a " + user.ScreenName);
            }
            else
            {
                MessageBox.Show("Has dejado de seguir a " + user.ScreenName);
            }
        }

        private void FollowFollower_Click(object sender, RoutedEventArgs e)
        {
            Follow(((UserView)FollowersContainer.SelectedItem).User);
        }

        private void UnFollowFollowing_Click(object sender, RoutedEventArgs e)
        {
            Unfollow(((UserView)FriendsContainer.SelectedItem).User);
        }

        private void UnFollowTraitor_Click(object sender, RoutedEventArgs e)
        {
            Unfollow(((UserView)TraitorsContainer.SelectedItem).User);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            FollowFollower.IsEnabled = false;
            UnFollowTraitor.IsEnabled = false;
            UnFollowFollowing.IsEnabled = false;

            // Fill the containers
            new Thread(new ThreadStart(() => LoadData())).Start();
        }
    }
}
