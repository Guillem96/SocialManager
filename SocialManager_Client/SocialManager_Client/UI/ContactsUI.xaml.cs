using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// Interaction logic for Contacts.xaml
    /// </summary>
    public partial class ContactsUI : UserControl
    {
        class RequestsView
        {
            public string FirstName { get => profile.FirstName; }
            public string LastName { get => profile.LastName; }
            public string Email { get => profile.Email; }
            public string Username { get => profile.Username; }

            private Profile profile;

            public RequestsView(Profile p)
            {
                profile = p;
            }

            public Profile GetProfile()
            {
                return profile;
            }

            public override string ToString()
            {
                return Username;
            }
        }

        public Timer setContacts;

        public ContactsUI()
        {
            InitializeComponent();

            // Datagrid to read only
            PossibleRequests.IsReadOnly = true;
            InboxRequests.IsReadOnly = true;
            SentRequests.IsReadOnly = true;

            // Set images sources
            AcceptImage.Source = PathUtilities.GetImageSource("accept.png");
            DenyImage.Source = PathUtilities.GetImageSource("deny.png");

            // Set contacts 
            setContacts = new Timer()
            {
                Interval = 1000, // Every 5 seconds
                Enabled = true
            };
            setContacts.Elapsed += (o, ev) => SetContacts();
        }

        public void Close()
        {
            setContacts.Enabled = false;
            setContacts.Dispose();
        }

        private void SetContacts()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Set contacts
                if (!ClientController.client.GetContactRequestList( out string message))
                {
                    MessageBox.Show(message);
                }
                else
                {
                    ContactsItems.Items.Clear();
                    // For each acontact create an entry to the list view
                    foreach (var contact in ClientController.client.Profile.Contacts)
                    {
                        // Image that represents the state of contact(online, offline)
                        Image con = new Image()
                        {
                            Source = PathUtilities.GetImageSource("connected.png"),
                            Width = 10,
                            Height = 10,
                            Margin = new Thickness(10, 2, 0, 0)
                        };

                        Image dis = new Image()
                        {
                            Source = PathUtilities.GetImageSource("disconnected.png"),
                            Width = 10,
                            Height = 10,
                            Margin = new Thickness(10, 2, 0, 0)
                        };

                        // Prettify image
                        Image contactImage = new Image()
                        {
                            Source = PathUtilities.GetImageSource("Profile.png"),
                            Width = 20,
                            Height = 20,
                            Margin = new Thickness(5, 2, 0, 0)
                        };

                        StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                        sp.Children.Add(contactImage);

                        // Contact username
                        sp.Children.Add(new Label()
                        {
                            Content = contact.Profile.Username,
                            Margin = new Thickness(10, 2, 0, 0)
                        });

                        sp.Children.Add(contact.Stat != Contact.Status.Disconnected ? con : dis);

                        ContactsItems.Items.Add(sp);
                    }
                }

                // Set the friend requests
                InboxRequests.ItemsSource =
                    ClientController.client.Profile.RecievedContactRequests
                                            .Select(c => new RequestsView(c.From));
                SentRequests.ItemsSource =
                    ClientController.client.Profile.SentContactRequests
                                            .Select(c => new RequestsView(c.To));
            }));
        }

        // Enable and disable some buttons
        private void InboxRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InboxRequests.SelectedItem == null)
            {
                AcceptButton.IsEnabled = false;
                DenyButton.IsEnabled = false;
                ViewProfileButtonRequest.IsEnabled = false;
                return;
            }

            AcceptButton.IsEnabled = true;
            DenyButton.IsEnabled = true;
            ViewProfileButtonRequest.IsEnabled = true;
        }

        // Make a client query and get the result
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // No quary, then nothing
            if (Query.Text == "")
                return;

            // Check that query is a unique word (else server problems)
            if(Query.Text.Split(new char[] { ' ' }).Length > 1)
            {
                MessageBox.Show("Search query must be a unique word.");
                return;
            }

            // Get the result
            List<Profile> profiles = null;

            if (!ClientController.ClientQuery(Query.Text, ref profiles, out string message))
            {
                MessageBox.Show(message);
                return;
            }

            // All correct
            PossibleRequests.ItemsSource = profiles.Select( c => new RequestsView(c));
        }

        // Send a friend request
        private void SendContactRequestButton_Click(object sender, RoutedEventArgs e)
        {
            // Send the contact request
            if (!ClientController.SendContactRequest(((RequestsView)PossibleRequests.SelectedItem).Username, out string message))
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("Contact request sended correctly to " + ((RequestsView)PossibleRequests.SelectedItem).Username);
            }

        }

        // Enable and disable some buttons 
        private void PossibleRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PossibleRequests.SelectedItem == null)
            {
                SendContactRequestButton.IsEnabled = false;

                return;
            }

            SendContactRequestButton.IsEnabled = true;
        }

        // Enable and disable some buttons
        private void ContactsItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContactsItems.SelectedItem == null)
            {
                ViewProfileButtonContact.IsEnabled = false;
                return;
            }

            ViewProfileButtonContact.IsEnabled = true;
        }

        // Open profile window
        private void ViewProfileButtonRequest_Click(object sender, RoutedEventArgs e)
        {
            Profile p = ((RequestsView)InboxRequests.SelectedItem).GetProfile();
            new ProfileView(p, "Unknown").ShowDialog();
        }

        // Open profile of a friend
        private void ViewProfileButtonContact_Click(object sender, RoutedEventArgs e)
        {
            string contactUsername = "";
            foreach(var child in ((StackPanel)ContactsItems.SelectedItem).Children)
            {
                if(child.GetType() == typeof(Label))
                {
                    contactUsername = ((Label)child).Content.ToString();
                    break;
                }
            }
            Contact p = ClientController.client.Profile.Contacts.Single(c => c.Profile.Username == contactUsername);
            new ProfileView(p.Profile, p.Stat.ToString()).ShowDialog();
        }

        // Accept the request
        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ClientController.AcceptRequest(
            ((RequestsView)InboxRequests.SelectedItem).Username, out string message))
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("Has agregado a " + ((RequestsView)InboxRequests.SelectedItem).Username + "!");
            }
        }

        // Deny the request
        private void DenyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ClientController.DenyRequest(
            ((RequestsView)InboxRequests.SelectedItem).Username, out string message))
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("Has rechazado a " + ((RequestsView)InboxRequests.SelectedItem).Username + "!");
            }
        }
    }
}
