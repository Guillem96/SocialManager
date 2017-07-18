

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        private Brush def;

        public SignUpWindow()
        {
            InitializeComponent();
            logoImage.Source = PathUtilities.GetImageSource("Logo.png");
            def = FirstName.BorderBrush;
        }

        private void Confimr_Click(object sender, RoutedEventArgs e)
        {

            if (FirstName.Text == "")
            {
                FirstName.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("First name is mandatory.");
                return;
            }
            else
                FirstName.BorderBrush = def;


            if (LastName.Text == "")
            {
                LastName.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Last name is mandatory.");
                return;
            }
            else
                LastName.BorderBrush = def;

            if (Age.Text == "")
            {
                Age.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Last name is mandatory.");
                return;
            }
            else
                Age.BorderBrush = def;

            if (!Male.IsChecked.Value && !Female.IsChecked.Value)
            {
                MessageBox.Show("Gender is mandatory.");
                return;
            }

            if (Username.Text == "")
            {
                Username.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Password is mandatory.");
                return;
            }
            else
                Username.BorderBrush = def;

            if (Password.Text == "")
            {
                Password.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Password is mandatory.");
                return;
            }
            else
                Password.BorderBrush = def;

            if (RPassword.Text == "")
            {
                RPassword.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Repeat Password is mandatory.");
                return;
            }
            else
                RPassword.BorderBrush = def;

            if (!RPassword.Text.Equals(Password.Text))
            {
                RPassword.BorderBrush = System.Windows.Media.Brushes.Red;
                Password.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Password incorrect.");
                return;
            }
            else
            {
                Password.BorderBrush = def;
                RPassword.BorderBrush = def;
            }

            int age = 0;
            try
            {
                age = int.Parse(Age.Text);
            }
            catch (FormatException)
            {
                Age.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Age must be an integer.");
                return;
            }
            
            // All correct, register client
            Profile p = new Profile(FirstName.Text, LastName.Text, age, PhoneNumber.Text,
                                    (Male.IsChecked.Value ? Profile.Sex.Male : Profile.Sex.Female),
                                    Username.Text, Password.Text, Email.Text, new List<Contact>());

            ClientController.client = new Client();
            string message = "";
            if(!ClientController.Register(p, out message))
            {
                MessageBox.Show(message);
            }
            else
            {
                MessageBox.Show(message);
                this.Close();
            }

        }
    }
}
