using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            SetContacts();
        }

        private void SetContacts()
        {
            string message = "";

            if (!ClientController.GetContactRequests(out message))
            {
                MessageBox.Show(message);
            }
            else
            {
                
                foreach (var contact in ClientController.client.Profile.Contacts)
                {
                    Image con = new Image() { Source = PathUtilities.GetImageSource("connected.png"), Width = 10, Height = 10, Margin = new Thickness(10, 2, 0, 0) };
                    Image dis = new Image() { Source = PathUtilities.GetImageSource("disconnected.png"), Width = 10, Height = 10, Margin = new Thickness(10, 2, 0, 0) };
                    Image noImageProfile = new Image() { Source = PathUtilities.GetImageSource("Profile.png"), Width = 20, Height = 20, Margin = new Thickness(5, 2, 0, 0) };

                    StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                    sp.Children.Add(noImageProfile);

                    sp.Children.Add(new Label() { Content = contact.Profile.Username, Margin = new Thickness(10, 2, 0, 0) });
                    sp.Children.Add(contact.Stat != Contact.Status.Disconnected ? con : dis);
                    ContactsItems.Items.Add(sp);
                }

                InboxRequests.ItemsSource = 
                    ClientController.client.Profile.RecievedContactRequests
                                            .Select(c => new RequestsView(c.From));
                SentRequests.ItemsSource = 
                    ClientController.client.Profile.SentContactRequests
                                            .Select(c => new RequestsView(c.To));
            }
        }

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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (Query.Text == "")
                return;
            if(Query.Text.Split(new char[] { ' ' }).Length > 1)
            {
                MessageBox.Show("Search query must be a unique word.");
                return;
            }
            string message = "";
            List<Profile> profiles = null;

            if(!ClientController.ClientQuery(Query.Text, ref profiles, out message))
            {
                MessageBox.Show(message);
                return;
            }

            // All correct
            PossibleRequests.ItemsSource = profiles.Select( c => new RequestsView(c));
        }

        private void SendContactRequestButton_Click(object sender, RoutedEventArgs e)
        {
            // Send the contact request
            string message = "";
            if(!ClientController.SendContactRequest(((RequestsView)PossibleRequests.SelectedItem).Username, out message))
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("Contact request sended correctly to " + ((RequestsView)PossibleRequests.SelectedItem).Username);
                SetContacts();
            }

        }

        private void PossibleRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PossibleRequests.SelectedItem == null)
            {
                SendContactRequestButton.IsEnabled = false;

                return;
            }

            SendContactRequestButton.IsEnabled = true;
        }

        private void ContactsItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContactsItems.SelectedItem == null)
            {
                ViewProfileButtonContact.IsEnabled = false;
                return;
            }

            ViewProfileButtonContact.IsEnabled = true;
        }

        private void ViewProfileButtonRequest_Click(object sender, RoutedEventArgs e)
        {
            new ProfileView(((RequestsView)InboxRequests.SelectedItem).GetProfile(), "Unknown").ShowDialog();
        }

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
            new ProfileView(p.Profile, "Unknown").ShowDialog();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            string message = "";
            if(!ClientController.AcceptRequest(
                        ((RequestsView)InboxRequests.SelectedItem).Username, out message))
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("Has agregado a " + ((RequestsView)InboxRequests.SelectedItem).Username + "!");
                SetContacts();
            }
        }

        private void DenyButton_Click(object sender, RoutedEventArgs e)
        {
            string message = "";
            if (!ClientController.DenyRequest(
                        ((RequestsView)InboxRequests.SelectedItem).Username, out message))
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("Has rechazado a " + ((RequestsView)InboxRequests.SelectedItem).Username + "!");
                SetContacts();
            }
        }
    }
}
