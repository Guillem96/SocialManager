using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        class PossibleRequestInfo
        {
            public string Username { get; set; }
            public override string ToString()
            {
                return Username;
            }
        }

        public ContactsUI()
        {
            InitializeComponent();

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
                Image con = new Image() { Source = PathUtilities.GetImageSource("connected.png"), Width = 20, Height = 20, Margin = new Thickness(5, 2, 0, 0) };
                Image dis = new Image() { Source = PathUtilities.GetImageSource("disconnected.png"), Width = 20, Height = 20, Margin = new Thickness(5, 2, 0, 0) };

                foreach (var contact in ClientController.client.Profile.Contacts)
                {
                    StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                    sp.Children.Add(new Label() { Content = contact.Profile.Username });
                    sp.Children.Add(contact.Stat != Contact.Status.Disconnected ? con : dis);
                    ContactsItems.Items.Add(sp);
                }

                InboxRequests.ItemsSource = ClientController.client.Profile.RecievedContactRequests.Select(c => new { From = c.From.Username });
                SentRequests.ItemsSource = ClientController.client.Profile.SentContactRequests.Select(c => new { To = c.To.Username });
            }
        }

        private void InboxRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InboxRequests.SelectedItem == null)
            {
                AcceptButton.IsEnabled = false;
                DenyButton.IsEnabled = false;
                return;
            }

            AcceptButton.IsEnabled = true;
            DenyButton.IsEnabled = true;
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
            List<string> usernames = null;

            if(!ClientController.ClientQuery(Query.Text, ref usernames, out message))
            {
                MessageBox.Show(message);
                return;
            }

            // All correct
            PossibleRequests.ItemsSource = usernames.Select(c => new PossibleRequestInfo() { Username = c});
        }

        
        private void SendContactRequestButton_Click(object sender, RoutedEventArgs e)
        {
            // Send the contact request
            string message = "";
            if(!ClientController.SendContactRequest(((PossibleRequestInfo)PossibleRequests.SelectedItem).Username, out message))
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("Contact request sended correctly to " + ((PossibleRequestInfo)PossibleRequests.SelectedItem).Username);
                SetContacts();
            }

        }

        private void PossibleRequests_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (PossibleRequests.SelectedItem == null)
            {
                SendContactRequestButton.IsEnabled = false;

                return;
            }

            SendContactRequestButton.IsEnabled = true;
        }
    }
}
