using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncTcpServer.Observer
{
    public interface ISubscriber
    {
        void AddClient(string username, TcpClient client);
        void RemoveClient(string username);
        void BroadcastToOthers(string msg, string username);
    }
}
