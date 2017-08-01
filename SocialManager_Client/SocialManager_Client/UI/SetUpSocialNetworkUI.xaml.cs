using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using WpfAnimatedGif;

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for SetUpSocialNetworkUI.xaml
    /// </summary>
    public partial class SetUpSocialNetworkUI : UserControl
    {
        string twitterGrid;
        string instagramGrid;

        private double loadingGridWidth;
        private double loadingGridHeight;

        private SocialNetwork instagram;


        public SetUpSocialNetworkUI()
        {
            InitializeComponent();

            ClientController.client.TwitterLogin();

            // Set images
            TwiterLogo.Source = PathUtilities.GetImageSource("twitter.png");
            TwiterLogo1.Source = PathUtilities.GetImageSource("twitter.png");
            InstagramLogo.Source = PathUtilities.GetImageSource("instagram.png");
            InstagramLogo1.Source = PathUtilities.GetImageSource("instagram.png");

            // Save the default form
            twitterGrid = XamlWriter.Save(TwitterGrid);
            instagramGrid = XamlWriter.Save(InstagramGrid);

            // Clear the old grid
            TwitterGrid.Children.Clear();
            InstagramGrid.Children.Clear();

            // Set loading grid sizes
            loadingGridWidth = LoadingGrid.Width;
            loadingGridHeight = LoadingGrid.Height;

            new Thread(() => LoadData("All")).Start();
        }

        public void LoadData(string socialNet)
        {

            Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingGridHeight, loadingGridWidth, LoadingGrid, "Cargando cuentas...")));

            CheckLinkController(socialNet);

            Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGrid)));
        }

        private void CheckLinkedSocialNets(string name, GroupBox parent, string defaultGrid, StackPanel linkMessage)
        {
            // Server request to update profile in case new social net is linked
            if (!ClientController.client.UpdateProfile(ClientController.client.Profile, out string message))
            {
                MessageBox.Show(message + "No se pueden cargar tus vínculos a las redes sociales.");
                return;
            }

            // If social net is linked
            if (ClientController.client.Profile.SocialNets.Any(c => c.Name.Equals(name)))
            {
                string imageUrl = "";
                if (name == "Twitter")
                {
                    ClientController.client.TwitterLogin();
                    ClientController.client.Twitter.Login();
                    imageUrl = ClientController.client.Twitter.GetProfileImage();
                }
                else
                {
                    ClientController.client.InstagramLogin();
                    ClientController.client.Instagram.Login();
                    imageUrl = ClientController.client.Instagram.GetProfileImageUrl();
                }


                // UI Actions
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(() =>
               {
                   // Social net is already set up
                   ((Grid)parent.Content).Children.Clear();

                   StackPanel sp = new StackPanel()
                   {
                       Orientation = Orientation.Horizontal
                   };

                   // Label to inform abaout the link exists
                   sp.Children.Add(new Label()
                   {
                       Content = name + " ya está vinculado.",
                       VerticalContentAlignment = VerticalAlignment.Center,
                       Margin = new Thickness(5, 0, 20, 0),
                   });

                   // Button to delete the link
                   Button b = new Button()
                   {
                       Content = "Eliminar vínculo.",
                       Height = 30,
                       Width = 120
                   };

                   b.Click += (o, e) =>
                   {
                       DeleteLink(name);
                   };

                   sp.Children.Add(b);

                   ((Grid)parent.Content).Children.Add(sp);

                   // Set the message to give some feedback to user
                   linkMessage.Children.Clear();

                   // State of account
                   linkMessage.Children.Add(new Label()
                   {
                       Content = name + " vinculado con la cuenta ",
                       VerticalAlignment = VerticalAlignment.Center
                   });

                   // Link to account
                   var link = new Hyperlink(new Run(name.Equals("Twitter") ? ClientController.client.Twitter.Username : ClientController.client.Instagram.Username))
                   {
                       NavigateUri = name.Equals("Twitter")
                               ? new Uri("https://twitter.com/" + ClientController.client.Twitter.Username)
                               : new Uri("https://instagram.com/" + ClientController.client.Instagram.Username),
                   };

                   // Open the account uri on an externar browser
                   link.RequestNavigate += (o, e) =>
                   {
                       e.Handled = true;
                       Process.Start(e.Uri.ToString());
                   };

                   Label href = new Label()
                   {
                       Content = link,
                       VerticalAlignment = VerticalAlignment.Center
                   };

                   linkMessage.Children.Add(href);


                   if (name.Equals("Twitter"))
                       AddTwitterImage(imageUrl);
                   else
                       AddInstagramImage(imageUrl);
               }));
                
            }
            else
            {
                var sb = new StringReader(defaultGrid);
                var xml = XmlReader.Create(sb);

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send,new Action(() =>
               {
                    // And set the default grid as a child of the parent 
                   parent.Content = null;
                   parent.Content = XamlReader.Load(xml) as Grid;
                   Button b = ((Grid)parent.Content).Children.OfType<Button>().FirstOrDefault() as Button;
                   PasswordBox pb = ((Grid)parent.Content).Children.OfType<PasswordBox>().FirstOrDefault() as PasswordBox;
                   TextBox ub = ((Grid)parent.Content).Children.OfType<TextBox>().FirstOrDefault() as TextBox;

                   b.Click += (o, s) => NewLink(name, ub, pb);

                   linkMessage.Children.Clear();

                   linkMessage.Children.Add(new Label()
                   {
                       Content = "No tienes ninguna cuenta vinculada a " + name
                   });

                   TwitterProfileImage.Fill = Brushes.Transparent;
                   TwitterProfileImage.Stroke = Brushes.Transparent;

                   InstagramProfileImage.Fill = Brushes.Transparent;
                   InstagramProfileImage.Stroke = Brushes.Transparent;

               }));

            }
        }

        private void NewLink(string socialNet, TextBox ub, PasswordBox pb)
        {
            SocialNetwork sn = new SocialNetwork()
            {
                Name = socialNet,
                Username = ub.Text,
                Password = pb.Password
            };

            new Thread(() =>
            {
                Dispatcher.BeginInvoke(new Action(() => Loading.StartLoading(loadingGridHeight, loadingGridWidth, LoadingGrid, "Comprobando cuenta de " + socialNet + "...")));

                // Check if login is correct
                if (socialNet.Equals("Twitter"))
                {
                    // Try to login
                    if (!new SocialNetworksLogic.Twitter(sn.Username, sn.Password).Login())
                    {
                        MessageBox.Show("Cuenta de twitter incorrecta.");
                        Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGrid)));

                        return;
                    }
                    else
                    {
                        if (!ClientController.client.LinkNewSocialNetwork(sn, out string message))
                            MessageBox.Show(message);
                        else
                            MessageBox.Show("Se ha vinculado a " + socialNet + " correctamente.");

                        Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGrid)));

                        LoadData("Twitter");
                    }
                }
                else
                {
                    if(!new SocialNetworksLogic.Instagram(sn.Username, sn.Password).Login())
                    {
                        MessageBox.Show("Cuenta de instagram incorrecta.");
                        Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGrid)));

                        return;
                    }
                    else
                    {
                        if (!ClientController.client.LinkNewSocialNetwork(sn, out string message))
                            MessageBox.Show(message);
                        else
                            MessageBox.Show("Se ha vinculado a " + socialNet + " correctamente.");

                        Dispatcher.BeginInvoke(new Action(() => Loading.EndLoading(LoadingGrid)));

                        LoadData("Instagram");
                    }
                }
            }).Start();
        }

        private void DeleteLink(string socialNet)
        {
            var sn = ClientController.client.Profile.SocialNets
                                    .Single(c => c.Name.Equals(socialNet));


            if(sn.Name == "Twitter")
            {
                ClientController.client.TwitterLogout();
            }

            if(!ClientController.client.DeleteLinkNewSocialNetwork( sn, out string message))
                MessageBox.Show(message);
            else
                MessageBox.Show(socialNet + " ha sido desvinculada.");


            new Thread(() => LoadData("All")).Start();
        }


        // Call the specific method depending on the social net
        private void CheckLinkController(string name)
        {

            switch (name)
            {
                case "Twitter":
                    CheckLinkedSocialNets("Twitter", TwitterBox, twitterGrid, TwitterLinkMessage);
                    break;
                case "Instagram":
                    CheckLinkedSocialNets("Instagram", InstagramBox, instagramGrid, InstagramLinkMessage);

                    break;
                case "All":
                    CheckLinkedSocialNets("Instagram", InstagramBox, instagramGrid, InstagramLinkMessage);
                    CheckLinkedSocialNets("Twitter", TwitterBox, twitterGrid, TwitterLinkMessage);
                    break;
            }
        }

        private void AddTwitterImage(string url)
        {
            TwitterProfileImage.Fill = new ImageBrush()
            {
                ImageSource = PathUtilities.GetImageSourceFromUri(url)
            };
        }

        private void AddInstagramImage(string url)
        {
            InstagramProfileImage.Fill = new ImageBrush()
            {
                ImageSource = PathUtilities.GetImageSourceFromUri(url)
            };
        }
    }
}
