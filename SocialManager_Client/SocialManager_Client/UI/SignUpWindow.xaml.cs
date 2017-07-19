

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
                MessageBox.Show("El Nombre es obligatorio.");
                return;
            }
            else
                FirstName.BorderBrush = def;


            if (LastName.Text == "")
            {
                LastName.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Los apellidos son obligatorios.");
                return;
            }
            else
                LastName.BorderBrush = def;

            if (Age.Text == "")
            {
                Age.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("La edad es obligatoria");
                return;
            }
            else
                Age.BorderBrush = def;

            if (!Male.IsChecked.Value && !Female.IsChecked.Value)
            {
                MessageBox.Show("El Sexo es obligatorio.");
                return;
            }

            if (Username.Text == "")
            {
                Username.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("El usuario es obligatorio.");
                return;
            }
            else
                Username.BorderBrush = def;

            if (Password.Password == "")
            {
                Password.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("La contraseña es obligatoria.");
                return;
            }
            else
                Password.BorderBrush = def;

            if (RPassword.Password == "")
            {
                RPassword.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Es necesario repetir la contraseña.");
                return;
            }
            else
                RPassword.BorderBrush = def;

            if (!RPassword.Password.Equals(Password.Password))
            {
                RPassword.BorderBrush = System.Windows.Media.Brushes.Red;
                Password.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Contraseña incorrecta.");
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
                MessageBox.Show("La edad tiene que ser un entero.");
                return;
            }

            //// All correct, register client
            Profile p = new Profile(FirstName.Text, LastName.Text, age, PhoneNumber.Text,
                                    (Male.IsChecked.Value ? Profile.Sex.Male : Profile.Sex.Female),
                                    Username.Text, Password.Password, Email.Text, new List<Contact>(), new List<Message>());

            ClientController.client = new Client();
            string message = "";
            if (!ClientController.Register(p, out message))
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
