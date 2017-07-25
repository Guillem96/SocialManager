using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : UserControl
    {
        private Contact to;

        public Chat(Contact to)
        {
            InitializeComponent();

            // Grid color random
            Random gen = new Random();
            maingrid.Background = new SolidColorBrush(Color.FromRgb((byte)gen.Next(100, 255), (byte)gen.Next(100, 255), (byte)gen.Next(100, 255)));
            
            // Set destination name
            this.to = to;
            Username.Text = to.Profile.Username;
            Username.IsReadOnly = true;

            // Offline message
            OfflineMessage.Text = to.Stat == Contact.Status.Disconnected ? to.Profile.Username + " is offline." : "";
            
            // Set image
            MessageImage.Source = PathUtilities.GetImageSource("newChat.png");

            // Get extern messages every second
            Timer getMessages = new Timer()
            {
                Enabled = true,
                Interval = 1000
            };
            getMessages.Elapsed += (o, e) => AddRecievedMessages();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // If no message introduced, then nothing
            if (MessageContent.Text == "") return;

            // Send the message and check if all is correct
            if (!ClientController.client.SendChatMessage(to.Profile.Username, MessageContent.Text, out string message))
            {
                // Error
                MessageBox.Show(message);
                return;
            }

            // Add the message to chat box
            AddMessage(ClientController.client.Profile.Username, MessageContent.Text);

            // Empty the message
            MessageContent.Text = "";
        }

        // Check new messages
        private void AddRecievedMessages()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Get messages sended from the current chat
                List<Message> messages = ClientController.client.Profile.Messages
                                                        .Where(m => m.From.Username == to.Profile.Username)
                                                        .ToList();

                // Add all messages
                foreach (var message in messages)
                    AddMessage(message.From.Username, message.Content);

                ClientController.client.Profile.Messages.RemoveAll(m => m.From.Username == to.Profile.Username);
            }));
        }

        // Add new text box to message container
        private void AddMessage(string who, string content)
        {
            TextBox tb = new TextBox()
            {
                TextWrapping = TextWrapping.Wrap,
                Text = who + ": " + content,
                Width = 150,
                Padding = new Thickness(10, 5, 5, 5),
                IsReadOnly = true
            };

            MessagesContainer.Items.Add(tb);
        }  
    }
}
