using System.Net.Sockets;

namespace AsyncTcpServer.MessageHandlers
{
    public interface IMessageHandler
    {
        Task HandleMessageAsync(NetworkStream stream);
    }
}
