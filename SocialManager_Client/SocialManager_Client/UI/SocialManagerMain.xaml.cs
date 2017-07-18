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
    /// Interaction logic for SocialManagerMain.xaml
    /// </summary>
    public partial class SocialManagerMain : Window
    {
        public SocialManagerMain()
        {
            InitializeComponent();
            logoImage.Source = PathUtilities.GetImageSource("Logo.png");
            LogoutImage.Source = PathUtilities.GetImageSource("Logout.png");
            EditProfileImage.Source= PathUtilities.GetImageSource("EditProfile.png");
            MainWindow.Title = "Social Manager - " + ClientController.client.Profile.Username; 
        }

        private void ContactsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ContactsUI());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            string message = "";
            if (!ClientController.client.Logout(out message))
                MessageBox.Show(message);
            else
            {
                this.Close();
                new LoginWindow().ShowDialog();
            }
        }
    }
}
