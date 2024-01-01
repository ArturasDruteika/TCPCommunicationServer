using System.Net.Sockets;
using System.Text;

namespace AsyncTcpServer.MessageHandlers
{
    public class MessageHandler : IMessageHandler
    {
        public async Task<int> HandleMessageAsync(NetworkStream stream, CancellationToken ctsToken)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ctsToken)) != 0)
                {
                    if (bytesRead == 0)
                    {
                        // Client has closed the connection
                        return -1;
                    }
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received: " + receivedData);
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }
    }
}
