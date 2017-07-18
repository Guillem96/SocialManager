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
            def = Username.BorderBrush;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if(Username.Text == "")
            {
                Username.BorderBrush = Brushes.Red;
                return;
            }
            else
                Username.BorderBrush = def;

            if (Password.Text == "")
            {
                Password.BorderBrush = Brushes.Red;
                return;
            }
            else
                Password.BorderBrush = def;

            // If all correct do login
            string message = "";
            ClientController.client = new Client();

            if (!ClientController.Login(Username.Text, Password.Text, out message))
            {
                MessageBox.Show(message);
                return;
            }
            else
            {
                new SocialManagerMain().ShowDialog();
                this.Close();
            }


        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            Window w = new SignUpWindow();
            w.Show();
        }
    }
}
