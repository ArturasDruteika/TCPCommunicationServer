using AsyncTcpServer.Containers;
using System.Net.Sockets;

namespace AsyncTcpServer.MessageHandlers
{
    public interface IMessageHandler
    {
        Task<ClientStatus> HandleMessageAsync(NetworkStream stream, CancellationToken ctsToken);
    }
}
