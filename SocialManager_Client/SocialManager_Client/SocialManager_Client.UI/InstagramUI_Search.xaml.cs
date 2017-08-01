using InstagramGot.Models;
using SocialManager_Client.UI;
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

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for InstagramUI_Search.xaml
    /// </summary>
    public partial class InstagramUI_Search : UserControl
    {
        private SocialNetworksLogic.Instagram instagram;
        private IMinifiedUser userLoaded = null;
        private string prevQuery = "";

        // Waiting for search
        private double loadingWaitForUserWidth;
        private double loadingWaitForUserHeight;

        // Loading tweets greed
        private double loadingTweetsGridWidth;
        private double loadingTweetsHeight;

        // Loading profile image greed
        private double loadingImageGridWidth;
        private double loadingImageHeight;

        // Loading profile info
        private double loadingInfoGridWidth;
        private double loadingInfoHeight;

        internal InstagramUI_Search(SocialNetworksLogic.Instagram instagram)
        {
            InitializeComponent();

            this.instagram = instagram;

            // Set the buttons
            ViewButton.Content = new Image() { Source = PathUtilities.GetImageSource("view.png") };

            ViewButton.IsEnabled = false;

            Follow.Content = new Image() { Source = PathUtilities.GetImageSource("add.png") };
            Unfollow.Content = new Image() { Source = PathUtilities.GetImageSource("delete.png") };

            Follow.IsEnabled = false;
            Unfollow.IsEnabled = false;

            // Set grid size
            loadingWaitForUserWidth = LoadingGridWaitForUser.Width;
            loadingWaitForUserHeight = LoadingGridWaitForUser.Height;

            loadingTweetsGridWidth = LoadingGridTweets.Width;
            loadingTweetsHeight = LoadingGridTweets.Height;

            loadingImageGridWidth = LoadingProfileImage.Width;
            loadingImageHeight = LoadingProfileImage.Height;

            loadingInfoGridWidth = LoadingInfoGrid.Width;
            loadingInfoHeight = LoadingInfoGrid.Height;

            // Load Data
            new Thread(new ThreadStart(() => LoadData())).Start();
        }

        // Autocomplete search box
        private void UserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            new Thread(new ThreadStart(() =>
            {
                // Suggestions for searchbox
                var suggestions = new List<InstagramUI_Users.UserView>();

                // Text must be larger than 2 characters
                if (prevQuery.Length > 2)
                {
                    // Look for the suggestions
                    var users = instagram.SearchUsers(prevQuery);
                    
                    // Fill the list with suggestions
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                    {
                        if (UserSearch.Text == "") return;

                        foreach (var u in users)
                        {
                            suggestions.Add(new InstagramUI_Users.UserView()
                            {
                                User = u
                            });
                        }
                        UsersSuggestion.ItemsSource = null;

                        // If suggestions found
                        if (suggestions.Count > 0)
                        {
                            UsersSuggestion.ItemsSource = suggestions;
                            UsersSuggestion.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            UsersSuggestion.ItemsSource = null;
                            UsersSuggestion.Visibility = Visibility.Collapsed;
                        }
                        prevQuery = UserSearch.Text;

                    }));
                }
                else
                {
                    // No query larger than 2 characters yet
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                    {
                        if (UserSearch.Text == "") return;

                        UsersSuggestion.ItemsSource = null;
                        UsersSuggestion.Visibility = Visibility.Collapsed;
                        prevQuery = UserSearch.Text;

                    }));
                }
            })).Start();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            UserSearch.Text = "";

            UsersSuggestion.ItemsSource = null;
            UsersSuggestion.Visibility = Visibility.Collapsed;

            prevQuery = "";

            // Load data
            Thread t = new Thread(new ThreadStart(() => LoadData()));
            t.Start();
            t.Join();
        }

        private void LoadData()
        {
            if (userLoaded == null)
            {
                // Waiting for user query
                Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingWaitForUserHeight, loadingWaitForUserWidth, LoadingGridWaitForUser, "Esperando a la búsqueda...")));
                return;
            }
            else
            {
                // End loading for user query
                Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGridWaitForUser)));

                // Start loading user's tweets
                Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingTweetsHeight, loadingTweetsGridWidth, LoadingGridTweets, "Cargando tweets...", 70, 70)));
                new Thread(new ThreadStart(() =>
                {
                    if (!LoadMedia())
                    {
                        MessageBox.Show("Error cargando los tweets.");
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGridTweets)));
                    }
                })).Start();

                // Start loading user's profile image
                Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingImageHeight, loadingImageGridWidth, LoadingProfileImage, "Cargando imagen...", 50, 50)));
                new Thread(new ThreadStart(() =>
                {
                    if (!LoadImage())
                    {
                        MessageBox.Show("Error cargando la imagen de perfil");
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingProfileImage)));
                    }
                })).Start();

                // Start loading user's profile image
                Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingInfoHeight, loadingInfoGridWidth, LoadingInfoGrid, "Cargando info...", 50, 50)));
                new Thread(new ThreadStart(() =>
                {
                    if (!LoadInfo())
                    {
                        MessageBox.Show("Error cargando la información del perfil.");
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingInfoGrid)));
                    }
                })).Start();
            }
        }

        private bool LoadImage()
        {
            try
            {
                // Get the user
                IUser user = instagram.GetUserById(userLoaded.Id);

                if (user == null)
                    return false;

                string url = user.ProfilePictureUrl;

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                {
                    ProfileImage.Fill = new ImageBrush() { ImageSource = url == null ? null : PathUtilities.GetImageSourceFromUri(url) };
                }));

                SetRandomBackground();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Random background for main grid
        private void SetRandomBackground()
        {
            // Get a random media
            Random gen = new Random();

            // Get Media from user
            var media = instagram.GetRecentMedia(userLoaded.Id, 20);

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

        // Fill the media container
        private bool LoadMedia()
        {
            try
            {
                List<InstagramUI_Media.MediaView> mediaList = new List<InstagramUI_Media.MediaView>();

                // Get the user
                IUser user = instagram.GetUserById(userLoaded.Id);

                if (user == null)
                    return false;

                // Get the user's tweets
                var medias = instagram.GetRecentMedia(userLoaded.Id,10);

                // Get profile image url 
                string url = user.ProfilePictureUrl;

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                {
                    MediaOf.Content = "Publicaciones de " + user.Username;
                    foreach (IMedia t in medias)
                    {
                        mediaList.Add(new InstagramUI_Media.MediaView()
                        {
                            Media = t,
                        });
                    }
                    MediaContainer.ItemsSource = null;
                    MediaContainer.ItemsSource = mediaList;
                }));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private bool LoadInfo()
        {
            try
            {
                // Get the user
                IUser user = instagram.GetUserById(userLoaded.Id);

                if (user == null)
                    return false;

                IRelationship rd = instagram.GetFriendShipWith(user.Id);

                string name = user.FullName;
                int followersNum = user.FollowedBy;
                int followingNum = user.Follows;
                string description = user.Bio;

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                {
                    InfoAboutBox.Header = "Acerca de " + user.Username;
                    UserName.Content = name;
                    UserDescription.Text = description;
                    NumFollowers.Content = followersNum;
                    NumFollowing.Content = followingNum;

                    var link = new Hyperlink(new Run("@" + user.Username))
                    {
                        NavigateUri = new Uri("https://www.instagram.com/" + user.Username)
                    };

                    // Open the account uri on an externar browser
                    link.RequestNavigate += (o, e) =>
                    {
                        e.Handled = true;
                        Process.Start(e.Uri.ToString());
                    };

                    LinkToProfile.Content = link;

                    if (rd.OutgoingRelation == OutgoingRelationshipStatus.Follows)
                    {
                        RelationshipInfo.Content = "TE SIGUE";
                        Unfollow.IsEnabled = true;
                        Unfollow.Click += (s, e) => InstagramUI_Users.Unfollow(userLoaded, instagram);
                        Follow.IsEnabled = false;
                    }
                    else if(rd.OutgoingRelation == OutgoingRelationshipStatus.None)
                    {
                        if(rd.IngoingRelation == IngoingRelationshipStatus.RequestedBy)
                        {
                            RelationshipInfo.Content = "ESPERANDO TU RESPUESTA";

                            Follow.IsEnabled = true;
                            Follow.Click += (s, e) => InstagramUI_Users.Approve(userLoaded, instagram);
                            Unfollow.IsEnabled = false;

                            Unfollow.IsEnabled = true;
                            Unfollow.Click += (s, e) => InstagramUI_Users.Ignore(userLoaded, instagram);
                            Follow.IsEnabled = false;
                        }
                        else
                        {
                            Follow.IsEnabled = true;
                            Follow.Click += (s, e) => InstagramUI_Users.Follow(userLoaded, instagram);
                            Unfollow.IsEnabled = false;
                        }
                    }
                    else if (rd.OutgoingRelation == OutgoingRelationshipStatus.Requested)
                    {
                        RelationshipInfo.Content = "SOLICITUD ENVIADA";

                    }

                }));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Enable and disable buttons
        private void OwnTimeLineContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MediaContainer.SelectedItem == null)
            {
                ViewButton.IsEnabled = false;
            }
            else
            {
                ViewButton.IsEnabled = true;
            }
        }

        // Select the userloaded
        private void UsersSuggestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersSuggestion.SelectedItem != null)
            {
                userLoaded = (UsersSuggestion.SelectedItem as InstagramUI_Users.UserView).User;
                UserSearch.Text = (UsersSuggestion.SelectedItem as InstagramUI_Users.UserView).UserName;
            }
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            new InstagramUI_ViewMedia((MediaContainer.SelectedItem as InstagramUI_Media.MediaView).Media, instagram).Show();
        }
    }
}

