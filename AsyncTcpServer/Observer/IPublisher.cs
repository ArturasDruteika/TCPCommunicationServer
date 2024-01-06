using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncTcpServer.Observer
{
    public interface IPublisher
    {
        void Attach(ISubscriber subscriber);
        void Detach(ISubscriber subscriber);
        void OnBroadcastToOthers(string msg, string username);
    }
}
