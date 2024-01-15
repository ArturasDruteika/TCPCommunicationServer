using AsyncTcpServer.Containers;
using System.Net.Sockets;

namespace AsyncTcpServer.MessageHandlers.MessageReceivers
{
    public interface IMessageReceiver
    {
        ClientStatus ReceiveMsg(NetworkStream stream, string username);
    }
}
