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
    /// Interaction logic for AgendaUI.xaml
    /// </summary>
    public partial class AgendaUI : UserControl
    {
        private Brush def;
        private Timer checkAgenda;

        public AgendaUI()
        {
            InitializeComponent();

            def = EventName.BorderBrush;

            // Check events timer
            checkAgenda = new Timer()
            {
                Enabled = true,
                Interval = 1000,
            };
            checkAgenda.Elapsed += (o, s) => CheckEvents();
        }

        public void Close()
        {
            checkAgenda.Enabled = false;
        }

        // Add the new events to events container
        private void CheckEvents()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                EventsContainer.Items.Clear();

                foreach (var e in ClientController.client.Profile.AgendaEvents)
                {
                    StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };

                    // Prettify image
                    Image eventImg = new Image()
                    {
                        Source = PathUtilities.GetImageSource("event.png"),
                        Margin = new Thickness(10, 5, 10, 5),
                        Width = 40,
                        Height = 40,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    sp.Children.Add(eventImg);

                    // Event name
                    sp.Children.Add(new Label()
                    {
                        Content = e.EventName,
                        FontWeight = FontWeights.Bold,
                        FontSize = 14,
                        Width = 100,
                        VerticalAlignment = VerticalAlignment.Center
                    });

                    // Event description
                    sp.Children.Add(new TextBox()
                    {
                        Text = e.EventInfo,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Width = 200,
                        Height = 60,
                        IsReadOnly = true,
                        Margin = new Thickness(10, 5, 10, 5),
                        BorderBrush = Brushes.Transparent
                    });

                    // Event date
                    sp.Children.Add(new Label()
                    {
                        Content = e.Date.ToShortDateString(),
                        FontSize = 14,
                        Foreground = Brushes.Gray,
                        Width = 100,
                        VerticalAlignment = VerticalAlignment.Center
                    });

                    // Delete event button
                    Button b = new Button()
                    {
                        Width = 30,
                        Height = 30,
                        BorderBrush = Brushes.Transparent,
                        Background = Brushes.Transparent,
                        Content = new Image()
                        {
                            Source = PathUtilities.GetImageSource("deny.png")
                        }
                    };

                    sp.Children.Add(b);

                    b.Click += (s , ev) => DeleteEvent(e);

                    EventsContainer.Items.Add(sp);
                }
            }));
        }
       
        // Create new events
        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if all fields are filled
            if(EventName.Text == "")
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                EventName.BorderBrush = Brushes.Red;
                return;
            }
            else
                EventName.BorderBrush = def;

            if (EventInfo.Text == "")
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                EventInfo.BorderBrush = Brushes.Red;
                return;
            }
            else
                EventInfo.BorderBrush = def;

            if(EventDate.SelectedDate == null)
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                EventDate.BorderBrush = Brushes.Red;
                return;
            }
            else
                EventDate.BorderBrush = def;

            // If all correct create the event and send the request to server
            AgendaEvent aEvent = new AgendaEvent()
            {
                EventInfo = EventInfo.Text,
                EventName = EventName.Text,
                Date = EventDate.SelectedDate.Value
            };

            if (!ClientController.client.AddNewAgendaEvent(aEvent, out string message))
            {
                // Error
                MessageBox.Show(message);
                return;
            }
            else
            {
                // Nice
                MessageBox.Show("Evento añadido correctamente.");
            }

        }

        private void DeleteEvent(AgendaEvent agendaEvent)
        {
            if(!ClientController.client.DeleteAgendaEvent(agendaEvent, out string message))
            {
                MessageBox.Show(message);
                return;
            }
            else
            {
                MessageBox.Show("Evento eliminado correctamente.");
            }
        }
    }

    
}
