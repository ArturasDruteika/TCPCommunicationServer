using MultipleClientServer.Containers;
using System.Net.Sockets;

namespace MultipleClientServer.MessageHandlers.MessageReceivers
{
    public interface IMessageReceiver
    {
        ClientStatus ReceiveMsg(NetworkStream stream);
    }
}
