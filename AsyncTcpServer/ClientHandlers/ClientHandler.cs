using AsyncTcpServer.Containers;
using AsyncTcpServer.ImageHandlers;
using AsyncTcpServer.MessageHandlers;
using System.Net.Sockets;
using System.Text;


namespace AsyncTcpServer.ClientHandlers
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly IMessageHandler _messageHandler;
        private readonly IImageHandler _imageHandler;
        private string _imgDirPath;

        public ClientHandler(TcpClient client, IMessageHandler messageHandler, IImageHandler imageHandler, string imgDirPath)
        {
            _client = client;
            _messageHandler = messageHandler;
            _imageHandler = imageHandler;
            _imgDirPath = imgDirPath;
        }

        public async Task HandleClientAsync(CancellationToken ctsToken)
        {
            LogClientConnectionTime();
            NetworkStream stream = _client.GetStream();

            // Assuming you have a method to read the first message
            string initialMessage = await ReadInitialMessage(_client);
            string username = ParseUsername(initialMessage);
            PrintClientInfo(username);

            byte[] buffer = new byte[3]; // Adjusted to 3 bytes for the "IMG" header

            // Read the header
            await stream.ReadAsync(buffer, 0, buffer.Length);
            string header = Encoding.ASCII.GetString(buffer);

            ClientStatus res = 0;

            if (header == CommandTypes.MSG)
            {
                res = await _messageHandler.HandleMessageAsync(stream, ctsToken);
            }
            else if (header == CommandTypes.IMG)
            {
                await _imageHandler.HandleImageAsync(stream, _imgDirPath, ctsToken);
            }
            else
            {
                Console.WriteLine("Client didn't use the protocol...");
            }

            CloseClientIfDisconnected(res);
        }

        private string ParseUsername(string initialMessage)
        {
            if (string.IsNullOrEmpty(initialMessage))
            {
                return string.Empty;
            }

            const string usernamePrefix = CommandTypes.MSG + "USERNAME:";
            if (initialMessage.StartsWith(usernamePrefix))
            {
                return initialMessage.Substring(usernamePrefix.Length);
            }

            return string.Empty; // Return empty if the prefix is not found
        }

        private async Task<string> ReadInitialMessage(TcpClient client)
        {
            if (client == null || !client.Connected)
            {
                return string.Empty;
            }

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024]; // Adjust buffer size as needed
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public void LogClientConnectionTime()
        {
            DateTime now = DateTime.Now;
            Console.WriteLine("Client's time of connection': " + now);
        }

        private void CloseClient()
        {
            _client.Close();
            Console.WriteLine("Client has disconnected...");
        }

        private void CloseClientIfDisconnected(ClientStatus status)
        {
            if (status == ClientStatus.DISCONNECTED)
            {
                CloseClient();
            }
        }

        private void PrintClientInfo(string username)
        {
            if (_client.Connected)
            {
                Console.WriteLine("Client:");
                Console.WriteLine("  Local Endpoint: " + _client.Client.LocalEndPoint);
                Console.WriteLine("  Remote Endpoint: " + _client.Client.RemoteEndPoint);
                Console.WriteLine("  Username: " + username);
            }
            else
            {
                Console.WriteLine("Client not connected.");
            }
        }
    }

}
