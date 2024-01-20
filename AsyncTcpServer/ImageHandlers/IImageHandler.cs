using System.Net.Sockets;

namespace MultipleClientServer.ImageHandlers
{
    public interface IImageHandler
    {
        Task HandleImageAsync(NetworkStream stream, string imgPath, CancellationToken ctsToken);
    }
}
