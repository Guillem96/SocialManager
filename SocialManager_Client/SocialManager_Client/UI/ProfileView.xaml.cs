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
    /// Interaction logic for ProfileView.xaml
    /// </summary>
    public partial class ProfileView : Window
    {
        public ProfileView(Profile p, string status)
        {
            InitializeComponent();
            // Set group box titles
            ProfileWindow.Title = "Social Manager - " + p.Username + "'s profile";
            ProfileGroup.Header = p.Username + "'s profile";
            
            // Set main profile info
            Username.Text = p.Username;
            ProfileImage.Source = PathUtilities.GetImageSource("Profile.png");

            Status.Text = status;
            if(status == "Unknown")
                Status.Foreground = Brushes.Gray;
            else
                Status.Foreground = status == "Disconnected" ? Brushes.Red : Brushes.LawnGreen;

            // Set general information
            FirstName.Text = p.FirstName;
            LastName.Text = p.LastName;
            Age.Text = p.Age.ToString();
            Gender.Text = p.Gender.ToString();

            // Set Contact info
            Email.Text = p.Email ?? "";
            PhoneNumber.Text = p.PhoneNumber ?? "";

            // Set background color depending on gender
            ProfileWindow.Background = (Profile.Sex)p.Gender == Profile.Sex.Female ? Brushes.LightPink : Brushes.LightSkyBlue;
        }
    }
}
