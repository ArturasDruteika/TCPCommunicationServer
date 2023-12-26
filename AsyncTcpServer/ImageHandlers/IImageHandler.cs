using System.Net.Sockets;

namespace AsyncTcpServer.ImageHandlers
{
    public interface IImageHandler
    {
        Task HandleImageAsync(NetworkStream stream, string imgPath);
    }
}
