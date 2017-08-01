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
    /// Interaction logic for InstagramUI_Media.xaml
    /// </summary>
    public partial class InstagramUI_Media : UserControl
    {
        public class MediaView
        {
            // Media info
            public IMedia Media { get; set; }
            public object MediaImageUrl { get => PathUtilities.GetImageSourceFromUri(Media.ImageThumbnailUrl); set => Media.ImageThumbnailUrl = value.ToString(); }
            public string MediaText { get => Media.Text; set => Media.Text = value; }
            public int LikesCount { get => Media.LikesCount; set => Media.LikesCount = value; }
            public int CommentsCount { get => Media.CommentsCount; set => Media.CommentsCount = value; }
            public string LocationName { get => (Media.Location == null) ? "None" : Media.Location.Name; set => Media.Location.Name = value; }
            public string UserUri { get => "https://instagram.com/" + Media.CreatedBy.Username; set => UserUri = value; }
            public string Username { get => Media.CreatedBy.Username; set => Username = value; }
            public object ProfileImage { get => PathUtilities.GetImageSourceFromUri(Media.CreatedBy.ProfileImageUrl); set => ProfileImage = value; }

            // Media images
            public static object HeartImage { get; set; } = PathUtilities.GetImageSource("heart.png");
            public static object CommentImage { get; set; } = PathUtilities.GetImageSource("comment.png");
            public static object LocationImage { get; set; } = PathUtilities.GetImageSource("location.png");
        }

        private SocialNetworksLogic.Instagram instagram;

        private double gridHeight;
        private double gridWidth;

        internal InstagramUI_Media(SocialNetworksLogic.Instagram instagram)
        {
            InitializeComponent();

            // Reference to instagram
            this.instagram = instagram;

            // Grid size
            gridHeight = LoadingOwnMedia.Height;
            gridWidth = LoadingOwnMedia.Width;

            // Setup buttons
            ViewFollowsButton.Content = new Image() { Source = PathUtilities.GetImageSource("view.png") };
            ViewOwnButton.Content = new Image() { Source = PathUtilities.GetImageSource("view.png") };

            ViewOwnButton.IsEnabled = false;
            ViewFollowsButton.IsEnabled = false;

            // Load medias
            new Thread(new ThreadStart(() => LoadMedias())).Start();
        }

        private void LoadMedias()
        {
            // Start loading images
            Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(gridHeight, gridWidth, LoadingOwnMedia, "Cargando media...", 70, 70)));
            Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(gridHeight, gridWidth, LoadingFollowsMedia, "Cargando media...", 70, 70)));

            // Fill both listviews
            // If filled correctly end loading, else continue loading until the next refresh
            new Thread(new ThreadStart(() =>
            {
                if (FillMedia(OwnRecentMediaContainer, true))
                    Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingOwnMedia)));
            })).Start();

            new Thread(new ThreadStart(() => SetRandomBackground())).Start();

            new Thread(new ThreadStart(() =>
            {
                if (FillMedia(FollowsMediaContainer, false))
                    Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingFollowsMedia)));
            })).Start();
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

        // Fill the containers
        private bool FillMedia(ListView Container, bool own)
        {
            try
            {
                List<MediaView> mediaList = new List<MediaView>();

                List<IMedia> medias = null;

                if (own)
                    medias = instagram.GetRecentMedia();
                else
                {
                    medias = new List<IMedia>();
                    // Add 2 recent medias of each follower
                    foreach(var follow in instagram.GetFollows(10))
                        medias.AddRange(instagram.GetRecentMedia(follow.Id, 2));
                }


                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    if (own)
                        ProfileImage.Fill = new ImageBrush(PathUtilities.GetImageSourceFromUri(instagram.GetProfileImageUrl()));

                    int SelectedIndex = Container.SelectedIndex;
                    foreach (IMedia media in medias)
                    {
                        mediaList.Add(new MediaView()
                        {
                            Media = media
                        });
                    }
                    Container.ItemsSource = mediaList;
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

        // Enable and diable the buttons depending on the selection
        private void FollowsMediaContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FollowsMediaContainer.SelectedItem == null)
            {
                ViewFollowsButton.IsEnabled = false;
            }
            else
            {
                ViewFollowsButton.IsEnabled = true;
            }
        }

        private void OwnRecentMediaContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OwnRecentMediaContainer.SelectedItem == null)
            {
                ViewOwnButton.IsEnabled = false;
            }
            else
            {
                ViewOwnButton.IsEnabled = true;
            }
        }

        private void ViewOwnButton_Click(object sender, RoutedEventArgs e)
        {
            new InstagramUI_ViewMedia((OwnRecentMediaContainer.SelectedItem as MediaView).Media, instagram).Show();
        }

        private void ViewFollowsButton_Click(object sender, RoutedEventArgs e)
        {
            new InstagramUI_ViewMedia((FollowsMediaContainer.SelectedItem as MediaView).Media, instagram).Show();

        }
    }
}
