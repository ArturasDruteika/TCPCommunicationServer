using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncTcpServer.Observer
{
    public interface ClientRemoverPublisher
    {
        void Attach(ISubscriber subscriber);
        void Detach(ISubscriber subscriber);
        void OnRemoveClient(TcpClient client);
    }
}
