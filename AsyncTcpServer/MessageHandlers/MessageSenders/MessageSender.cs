using AsyncTcpServer.Containers;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Client.MessageHandlers.MessageSenders
{
    public class MessageSender : IMessageSender
    {
        public async Task SendMsg(string msg, string username, NetworkStream stream, CancellationToken cancellationToken)
        {
            byte[] dataToSend = CreateSendingData(msg, username);
            await stream.WriteAsync(dataToSend, 0, dataToSend.Length);
        }

        private static byte[] CreateSendingData(string msg, string username)
        {
            byte[] headerBytes = Encoding.ASCII.GetBytes(CommandTypes.MSG);
            byte[] usernameBytes = new byte[16];
            Encoding.ASCII.GetBytes(username).CopyTo(usernameBytes, 0);
            byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
            byte[] combinedMsg = CombineMessage(headerBytes, usernameBytes, msgBytes);

            return combinedMsg;
        }

        private static byte[] CombineMessage(byte[] headerBytes, byte[] usernameBytes, byte[] msgBytes)
        {
            byte[] combined = new byte[
                headerBytes.Length +
                usernameBytes.Length +
                msgBytes.Length
                ];

            Buffer.BlockCopy(headerBytes, 0, combined, 0, headerBytes.Length);
            Buffer.BlockCopy(usernameBytes, 0, combined, headerBytes.Length, usernameBytes.Length);
            Buffer.BlockCopy(msgBytes, 0, combined, headerBytes.Length + usernameBytes.Length, msgBytes.Length);
            
            return combined;
        }
    }
}
