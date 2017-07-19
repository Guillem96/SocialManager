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

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            bool changePassword = CurrentPassword.Password != "" && NewPassword.Password != "";
            string password;
            if (changePassword &&
                    CurrentPassword.Password == ClientController.client.Profile.Password)
                password = NewPassword.Password;
            else
                password = ClientController.client.Profile.Password;


            Profile p = new Profile(FirstName.Text,
                                        LastName.Text,
                                        int.Parse(Age.Text),
                                        PhoneNumber.Text,
                                        ClientController.client.Profile.Gender,
                                        Username.Text,
                                        password,
                                        Email.Text,
                                        ClientController.client.Profile.Contacts,
                                        ClientController.client.Profile.Messages);

            string message = "";
            if(!ClientController.client.UpdateProfile(p, out message))
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show("Perfil actualizado correctamente.");
                this.Close();
            }


        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
