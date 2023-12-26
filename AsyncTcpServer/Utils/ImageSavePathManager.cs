using System.Reflection;

namespace AsyncTcpServer.Utils
{
    internal class ImageSavePathManager
    {
        public static string GetImageSavePath()
        {
            string executablePath = Assembly.GetExecutingAssembly().Location;
            string executableDirectory = Path.GetDirectoryName(executablePath);
            string imageSavePath = Path.Combine(executableDirectory, "data", "images");
            Directory.CreateDirectory(imageSavePath);

            return imageSavePath;
        }
    }
}
