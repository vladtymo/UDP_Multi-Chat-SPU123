using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace udp_chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string serverIP = "127.0.0.1";
        const short serverPort = 3344;

        bool isListening = false;
        IPEndPoint serverEndPoint;
        UdpClient client = new UdpClient();

        public MainWindow()
        {
            InitializeComponent();

            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        }

        private async void Listen()
        {
            while (isListening)
            {
                try
                {
                    var result = await client.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(result.Buffer);

                    historyList.Items.Add(message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
        }
        private async void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(data, data.Length, serverEndPoint);
        }

        private void JoinMenuItemClick(object sender, RoutedEventArgs e)
        {
            SendMessage("<join>");

            if (!isListening)
            {
                isListening = true;
                Listen();
            }
        }

        private void LeaveMenuItemClick(object sender, RoutedEventArgs e)
        {
            SendMessage("<leave>");
            isListening = false;
        }

        private void SendBtnClick(object sender, RoutedEventArgs e)
        {
            string message = msgTextBox.Text;
            // TODO: validate message
            SendMessage(message);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SendMessage("<leave>");
            isListening = false;
        }
    }
}
