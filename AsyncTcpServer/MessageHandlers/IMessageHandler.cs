using System.Net.Sockets;

namespace AsyncTcpServer.MessageHandlers
{
    public interface IMessageHandler
    {
        Task<int> HandleMessageAsync(NetworkStream stream, CancellationToken ctsToken);
    }
}
