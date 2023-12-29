using System.Net.Sockets;
using System.Text;

namespace AsyncTcpServer.MessageHandlers
{
    public class MessageHandler : IMessageHandler
    {
        public async Task HandleMessageAsync(NetworkStream stream, CancellationToken ctsToken)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ctsToken)) != 0)
                {
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received: " + receivedData);

                    byte[] sendData = Encoding.UTF8.GetBytes(receivedData);
                    await stream.WriteAsync(sendData, 0, sendData.Length, ctsToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}
