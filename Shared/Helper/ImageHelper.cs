using System.Drawing;
using System.Net;
using System.Net.Sockets;

namespace Shared.Helper;

public class ImageHelper
{
    public ImageHelper()
    {
        var pathForServer = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Server");
        var pathForClient = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Client");

        if (!Directory.Exists(pathForServer))
        {
            Directory.CreateDirectory(pathForServer);
        }

        if (!Directory.Exists(pathForClient))
        {
            Directory.CreateDirectory(pathForClient);
        }

    }

    public string GetImagePath(byte[] buffer)
    {
        ImageConverter ic = new ImageConverter();
        var data = ic.ConvertFrom(buffer);


        Image img = data as Image;
        if (img != null)
        {
            Bitmap bitmap1 = new Bitmap(img);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Images2");
            Directory.CreateDirectory(path);
            var strGuid = Guid.NewGuid().ToString();
            bitmap1.Save($@"{path}\image{strGuid}.png");
            var imagepath = $@"{path}\image{strGuid}.png";
            return imagepath;
        }
        else
        {
            return String.Empty;
        }
    }

    public byte[] GetBytesOfImage(string path)
    {
        var image = new Bitmap(path);

        ImageConverter imageconverter = new ImageConverter();
        var imagebytes = ((byte[])imageconverter.ConvertTo(image, typeof(byte[])));

        return imagebytes;
    }

    public string TakeScreenShot(string folderName)
    {
        Bitmap bmp = new Bitmap(1920, 1080);
        string fullPath = string.Empty;

        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.CopyFromScreen(0, 0, 0, 0, new Size(1920, 1080));

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), folderName);

            fullPath = path + "\\screenshot_" + Guid.NewGuid().ToString() + ".png";

            bmp.Save(fullPath);  // saves the image
        }
        Console.WriteLine("Image has been saved to folder");
        return fullPath;
    }


    public void ChunkImage(byte[] imgBytes, UdpClient client, IPEndPoint ep)
    {
        var skipCount = 0;
        var maxValue = ushort.MaxValue - 28;
        var bytesLen = imgBytes.Length;

        if (bytesLen > maxValue)
        {
            while (skipCount + maxValue <= bytesLen)
            {
                client.Send(imgBytes
                    .Skip(skipCount)
                    .Take(maxValue)
                    .ToArray(), maxValue, ep);

                skipCount += maxValue;  
            }

            if (skipCount != bytesLen)
            {
                client.Send(imgBytes
                    .Skip(skipCount)
                    .Take(bytesLen - skipCount)
                    .ToArray(), bytesLen - skipCount, ep);
            }
        }
        else
        {
            client.Send(imgBytes, imgBytes.Length, ep);
        }
    } 
    public string SaveClientImage(byte[] data)
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Client", $"image_{Guid.NewGuid()}.png");
        File.WriteAllBytes(path, data);
        return path;
    }
}
