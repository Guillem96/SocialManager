using InstagramGot.Models;
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
using System.Windows.Threading;

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for InstagramUI_ViewMedia.xaml
    /// </summary>
    public partial class InstagramUI_ViewMedia : Window
    {
        private class CommentView
        {
            public IComment Comment { get; set; }
            public string Text { get => Comment.Text; set => Comment.Text = value; }
            public object ProfileImage { get => PathUtilities.GetImageSourceFromUri(Comment.From.ProfileImageUrl); set => ProfileImage = null; }
        }

        private IMedia media;
        private SocialNetworksLogic.Instagram instagram;

        internal InstagramUI_ViewMedia(IMedia media, SocialNetworksLogic.Instagram instagram)
        {
            InitializeComponent();

            this.media = media;
            this.instagram = instagram;

            MediaView.Title = "Social Manager - " + media.ToString();
            MediaView.Icon = PathUtilities.GetImageSource("Logo.png");

            // Set buttons
            CommentButton.Content = new Image() { Source = PathUtilities.GetImageSource("comment.png") };
            DeleteComment.Content = new Image() { Source = PathUtilities.GetImageSource("delete.png") };

            DeleteComment.IsEnabled = false;

            // Set background
            System.Drawing.Color color = Colors.ColorMath.GetDominantColor(media.ImageLowResolutionUrl);
            MainGrid.Background = new SolidColorBrush(Color.FromArgb(100, color.R, color.G, color.B));
            
            // Set images
            HeartImage.Source = PathUtilities.GetImageSource("heart.png");
            CommentImage.Source = PathUtilities.GetImageSource("comment.png");
            LocationImage.Source = PathUtilities.GetImageSource("location.png");

            // Set media UI
            MediaFile.Source = PathUtilities.GetImageSourceFromUri(media.ImageStandardResolutionUrl);
            LikesCount.Content = media.LikesCount.ToString();
            CommentsCount.Content = media.CommentsCount.ToString();

            LocationName.Content = (media.Location == null) ? "None" : media.Location.Name;

            ProfileImage.Fill = new ImageBrush(PathUtilities.GetImageSourceFromUri(media.CreatedBy.ProfileImageUrl));
            Username.Content = media.CreatedBy.Username;

            new Thread(new ThreadStart(() => FillComments())).Start();
        }

        private void FillComments()
        {
            List<IComment> comments = instagram.GetComments(media.Id);
            List<CommentView> commentsList = new List<CommentView>();

            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                foreach (var comment in comments)
                {
                    commentsList.Add(new CommentView()
                    {
                        Comment = comment
                    });
                }

                CommentsContainer.ItemsSource = commentsList;
            }));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (CommentText.Text == "") return;

            if(instagram.PostComment(media.Id, CommentText.Text))
            {
                MessageBox.Show("Comentario publicado correctamente.");
                new Thread(new ThreadStart(() => FillComments())).Start();
                CommentText.Text = "Escribe tu comentario...";
            }
            else
            {
                MessageBox.Show("Error al publicar el comentario.");
            }
        }

        private void DeleteComment_Click(object sender, RoutedEventArgs e)
        {
            if (instagram.DeleteComment(media.Id, (CommentsContainer.SelectedItem as CommentView).Comment.Id))
            {
                MessageBox.Show("Comentario eliminado correctamente.");
                new Thread(new ThreadStart(() => FillComments())).Start();
            }
            else
            {
                MessageBox.Show("Error al eliminar el comentario.");
            }
        }

        private void CommentsContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CommentsContainer.SelectedItem != null)
            {
                IComment c = (CommentsContainer.SelectedItem as CommentView).Comment;
                if (c.From.Username == instagram.Username)
                    DeleteComment.IsEnabled = true;
                else
                    DeleteComment.IsEnabled = false;

            }
            else
                DeleteComment.IsEnabled = false;

        }
    }
}
