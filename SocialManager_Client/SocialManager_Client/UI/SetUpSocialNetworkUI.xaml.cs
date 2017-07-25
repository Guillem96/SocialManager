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

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for SetUpSocialNetworkUI.xaml
    /// </summary>
    public partial class SetUpSocialNetworkUI : UserControl
    {
        string twitterGrid;
        string instagramGrid;

        private SocialNetworksLogic.Twitter twitter;
        private SocialNetwork instagram;


        public SetUpSocialNetworkUI()
        {
            InitializeComponent();

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

            // Check if social nets are linked
            new Thread(() => CheckLinkController("All")).Start();
        }

        /// <returns>True if social net is linked</returns>
        private bool CheckLinkedSocialNets(string name, GroupBox parent, string defaultGrid, StackPanel linkMessage)
        {
            bool isLinked = false;
            Dispatcher.Invoke(new Action(() =>
            {
                if (!ClientController.client.UpdateProfile(ClientController.client.Profile, out string message))
                {
                    MessageBox.Show(message + "No se pueden cargar tus vínculos a las redes sociales.");
                    return;
                }

                if (ClientController.client.Profile.SocialNets.Any(c => c.Name.Equals(name)))
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

                    // Get reference to the social network that be are checking
                    if (name == "Twitter")
                        twitter = ClientController.client.TwitterLogin();
                    else
                        instagram = ClientController.client.Profile.SocialNets.SingleOrDefault(c => c.Name == "Instagram");

                    // Set the message to give some feedback to user
                    linkMessage.Children.Clear();
                    
                    // State of account
                    linkMessage.Children.Add(new Label()
                    {
                        Content = name + " vinculado con la cuenta ",
                        VerticalAlignment = VerticalAlignment.Center
                    });

                    // Link to account
                    var link = new Hyperlink(new Run(twitter != null ? twitter.Username : instagram.Username))
                    {
                        NavigateUri = name.Equals("Twitter")
                                ? new Uri("https://twitter.com/" + twitter.Username)
                                : new Uri("https://instagram.com/" + instagram.Username),
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

                    isLinked = true;
                }
                else
                {
                    // And set the default grid as a child of the parent 
                    parent.Content = null;
                    var sb = new StringReader(defaultGrid);
                    var xml = XmlReader.Create(sb);
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

                }
            }), System.Windows.Threading.DispatcherPriority.Render);
            return isLinked;
        }

        private void NewLink(string socialNet, TextBox ub, PasswordBox pb)
        {
            SocialNetwork sn = new SocialNetwork()
            {
                Name = socialNet,
                Username = ub.Text,
                Password = pb.Password
            };

            // Check if login is correct
            if (socialNet.Equals("Twitter"))
            {
                // Try to login
                if(!new SocialNetworksLogic.Twitter(sn.Username, sn.Password).Login())
                {
                    MessageBox.Show("Cuenta de twitter incorrecta.");
                    return;
                }
            }

            if (!ClientController.client.LinkNewSocialNetwork(sn, out string message))
                MessageBox.Show(message);
            else
                MessageBox.Show("Se ha vinculado a "+ socialNet +" correctamente.");

            new Thread(() => CheckLinkController(socialNet)).Start();
        }

        private void DeleteLink(string socialNet)
        {
            var sn = ClientController.client.Profile.SocialNets
                                    .Single(c => c.Name.Equals(socialNet));
            if(!ClientController.client.DeleteLinkNewSocialNetwork( sn, out string message))
                MessageBox.Show(message);
            else
                MessageBox.Show(socialNet + " ha sido desvinculada.");
            

            new Thread(() => CheckLinkController(socialNet)).Start();
        }

        // Call the specific method depending on the social net
        private void CheckLinkController(string name)
        {
            switch (name)
            {
                case "Twitter":
                    if (CheckLinkedSocialNets("Twitter", TwitterBox, twitterGrid, TwitterLinkMessage))
                        AddTwitterImage();
                    break;
                case "Instagram":
                    CheckLinkedSocialNets("Instagram", InstagramBox, instagramGrid, InstagramLinkMessage);

                    break;
                case "All":
                    CheckLinkedSocialNets("Instagram", InstagramBox, instagramGrid, InstagramLinkMessage);
                    if (CheckLinkedSocialNets("Twitter", TwitterBox, twitterGrid, TwitterLinkMessage))
                        AddTwitterImage();
                    break;
            }

        }

        private void AddTwitterImage()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                SocialNetworksLogic.Twitter twitter = ClientController.client.TwitterLogin();
                twitter.Login();

                TwitterLinkMessage.Children.Add(new Ellipse()
                {
                    Width = 60,
                    Height = 60,
                    Margin = new Thickness(10, 0, 0, 0),
                    Fill = new ImageBrush()
                    {
                        ImageSource = PathUtilities.GetImageSourceFromUri(twitter.GetProfileImage())
                    }
                });
            }), System.Windows.Threading.DispatcherPriority.Render);
        }
    }
}
