using AsyncTcpServer.Containers;
using System.Net.Sockets;

namespace AsyncTcpServer.MessageHandlers.MessageReceivers
{
    public interface IMessageReceiver
    {
        Task<ClientStatus> HandleMessageAsync(NetworkStream stream, CancellationToken ctsToken, string username);
    }
}
