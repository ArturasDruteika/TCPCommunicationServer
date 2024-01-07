using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace AsyncTcpServer.ImageHandlers
{
    public class ImageHandler : IImageHandler
    {
        public async Task HandleImageAsync(NetworkStream stream, string imgPath, CancellationToken ctsToken)
        {
            string imgName = "received_img.jpg";
            imgPath = imgPath + "/" + imgName;
            // Specify the path and name for the saved image
            using var fileStream = new FileStream(imgPath, FileMode.Create);

            // Read the image data from the stream and write it to the file
            await stream.CopyToAsync(fileStream, ctsToken);

            Console.WriteLine($"Image saved to {imgPath}");
            // Further processing of the image...
        }
    }
}
