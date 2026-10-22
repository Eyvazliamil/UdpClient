using System.Net;
using System.Text;
using System.Net.Sockets;
using Shared.Helper;

class Program
{
    static void Main(string[] args)
    {
        var server = new UdpClient(45678);

        var remoteEP = new IPEndPoint(IPAddress.Any, 0);

        Console.WriteLine("Server started......");

        while (true)
        {
            var bytes = server.Receive(ref remoteEP);
            var str = Encoding.Default.GetString(bytes);
            Console.WriteLine(str);

            if (str == "Start")
            {
                var imageHelper = new ImageHelper();

                Console.WriteLine("Start server screenShot...");
                for (int i = 0; i < 24; i++)
                {
                    var imagePath = imageHelper.TakeScreenShot("Server");
                    byte[] imageBytes = imageHelper.GetBytesOfImage(imagePath);

                    imageHelper.ChunkImage(imageBytes, server, remoteEP);
                    Thread.Sleep(50);
                }
                Console.WriteLine("Server completed successfully message to Client...");
            }
        }
    }
}