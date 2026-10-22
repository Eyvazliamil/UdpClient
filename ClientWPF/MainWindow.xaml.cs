using Shared.Helper;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void StartBTN_Click(object sender, RoutedEventArgs e)
        {
            var client = new UdpClient();

            var endPoint = IPAddress.Parse("127.0.0.1");
            var port = 45678;

            var connectEP = new IPEndPoint(endPoint, port);

            MessageBox.Show("Client Started", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
             
            var startBytes = Encoding.Default.GetBytes("Start");
            client.Send(startBytes, startBytes.Length, connectEP);

            var imageHelper = new ImageHelper();

            await Task.Run(() =>
            {
                for (int i = 0; i < 24; i++)
                { 
                    var allBytes = new List<byte>();

                    while (true)
                    {
                        var data = client.Receive(ref connectEP);
                        allBytes.AddRange(data);
                         
                        if (data.Length < ushort.MaxValue - 28)
                            break;
                    }

                    imageHelper.SaveClientImage(allBytes.ToArray());
                }
            });

            MessageBox.Show("All images recieved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}