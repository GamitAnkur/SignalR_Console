
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows;


namespace Desktop_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection connection;

        public MainWindow()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7082/chathub")
                .WithAutomaticReconnect()
                .Build();

            /*-- Reconnecting --*/
            connection.Reconnecting += (sender) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = "Attempting to reconnect...";
                    messages.Items.Add(newMessage);

                });

                return Task.CompletedTask;
            };

            /*-- Reconnecting --*/
            connection.Reconnected += (sender) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = "Reconnected to server";
                    messages.Items.Clear();
                    messages.Items.Add(newMessage);

                });

                return Task.CompletedTask;
            };

            /*-- Closed --*/
            connection.Closed += (sender) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = "Connection closed ";
                    messages.Items.Add(newMessage);
                    openConnection.IsEnabled=true;
                    sendMessage.IsEnabled = false;

                });

                return Task.CompletedTask;
            };
        }

        /*-- Open connection btn --*/

        private  async void openConnection_Click(object sender , RoutedEventArgs e)
        {
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";
                    messages.Items.Add(newMessage);

                });

            });


            try
            {
                await connection.StartAsync();
                messages.Items.Add("connection Started");
                openConnection.IsEnabled = false;
                sendMessage.IsEnabled = true;
            }
            catch(Exception ex) { 
            
                messages.Items.Add($"{ex.Message}");
            }

        }

        /*-- send Message btn --*/

        private async void sendMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", "WPF Client", messageInput.Text);

            }

            catch(Exception ex)
            {

                messages.Items.Add(ex.Message);

            }
        }


    }
}
