using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for EditProfileWindow.xaml
    /// </summary>
    public partial class EditProfileWindow : Window
    {

        public EditProfileWindow()
        {
            InitializeComponent();

            // Set icon
            ProfileWindow.Icon = PathUtilities.GetImageSource("Logo.png");
            // Set information to the text box
            PhoneNumber.Text = ClientController.client.Profile.PhoneNumber;
            Email.Text = ClientController.client.Profile.Email;

            // Set main profile info
            Username.Text = ClientController.client.Profile.Username;
            ProfileImage.Source = PathUtilities.GetImageSource("Profile.png");

            // Set general information
            Profile p = ClientController.client.Profile;
            FirstName.Text = p.FirstName;
            LastName.Text = p.LastName;
            Age.Text = p.Age.ToString();
            Gender.Text = p.Gender.ToString();

        }

        // Accept the changes
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            // Check if password has been changed correctly
            bool changePassword = CurrentPassword.Password != "" && NewPassword.Password != "";
            string password;
            if (changePassword &&
                    CurrentPassword.Password == ClientController.client.Profile.Password)
                password = NewPassword.Password;
            else
                password = ClientController.client.Profile.Password;


            // Send the new profile
            Profile p = new Profile(FirstName.Text,
                                        LastName.Text,
                                        int.Parse(Age.Text),
                                        PhoneNumber.Text,
                                        ClientController.client.Profile.Gender,
                                        Username.Text,
                                        password,
                                        Email.Text,
                                        ClientController.client.Profile.Contacts,
                                        ClientController.client.Profile.Messages,
                                        ClientController.client.Profile.AgendaEvents,
                                        ClientController.client.Profile.SocialNets);

            if (!ClientController.client.UpdateProfile(p, out string message))
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("Perfil actualizado correctamente.");
                this.Close();
            }
        }

        // Cancel the changes
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
