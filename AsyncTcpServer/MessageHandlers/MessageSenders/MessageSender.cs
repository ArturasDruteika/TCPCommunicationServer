using AsyncTcpServer.Containers;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Client.MessageHandlers.MessageSenders
{
    public class MessageSender : IMessageSender
    {
        public async Task SendMsg(string msg, NetworkStream stream, CancellationToken cancellationToken)
        {
            byte[] header = Encoding.ASCII.GetBytes(CommandTypes.MSG);
            byte[] data = Encoding.ASCII.GetBytes(msg);

            // Create a new array that can hold both header and data
            byte[] combined = new byte[header.Length + data.Length];

            // Copy the header and data into the combined array
            Buffer.BlockCopy(header, 0, combined, 0, header.Length);
            Buffer.BlockCopy(data, 0, combined, header.Length, data.Length);

            // Send the combined message in one go
            await stream.WriteAsync(combined, 0, combined.Length);
        }
    }
}
