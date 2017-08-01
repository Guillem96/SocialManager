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

namespace SocialManager_Client.UI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        private Brush def;

        public LoginWindow()
        {
            InitializeComponent();
            logoImg.Source = PathUtilities.GetImageSource("Logo.png");
            Window.Icon = PathUtilities.GetImageSource("Logo.png");
            def = Username.BorderBrush;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Check that all fields are correct
            if(Username.Text == "")
            {
                Username.BorderBrush = Brushes.Red;
                return;
            }
            else
                Username.BorderBrush = def;

            if (Password.Password == "")
            {
                Password.BorderBrush = Brushes.Red;
                return;
            }
            else
                Password.BorderBrush = def;

            // If all correct do login
            ClientController.client = new Client();

            if (!ClientController.Login(Username.Text, Password.Password, out string message))
            {
                MessageBox.Show(message);
                return;
            }
            else
            {
                this.Close();
                new SocialManagerMain().ShowDialog();
            }
        }

        // Open register window
        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            Window w = new SignUpWindow();
            w.Show();
        }
    }
}
