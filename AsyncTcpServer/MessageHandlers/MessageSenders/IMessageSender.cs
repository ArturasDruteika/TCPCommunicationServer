﻿using System.IO;
using System.Net.Sockets;

namespace MultipleClientServer.MessageHandlers.MessageSenders
{
    public interface IMessageSender
    {
        public Task SendMsg(string msg, string username, NetworkStream stream, CancellationToken cancellationToken);
    }
}
