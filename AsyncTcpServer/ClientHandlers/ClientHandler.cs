using AsyncTcpServer.Containers;
using AsyncTcpServer.ImageHandlers;
using AsyncTcpServer.MessageHandlers.MessageReceivers;
using System.Net.Sockets;
using System.Text;


namespace AsyncTcpServer.ClientHandlers
{
    public class ClientHandler
    {
        private readonly TcpClient Client;
        private string Username = string.Empty;
        private readonly IMessageReceiver MessageHandler;
        private readonly IImageHandler ImageHandler;
        private string ImgDirPath;

        public delegate void NewClientHandler(string username, TcpClient client);
        public event NewClientHandler NewClient;
        public delegate void RemoveClientHandler(string username);
        public event RemoveClientHandler RemoveClient;

        public ClientHandler(TcpClient client, IMessageReceiver messageHandler, IImageHandler imageHandler, string imgDirPath)
        {
            Client = client;
            MessageHandler = messageHandler;
            ImageHandler = imageHandler;
            ImgDirPath = imgDirPath;
        }

        public void OnAddClient(string username, TcpClient client)
        {
            NewClient?.Invoke(username, client);
        }

        public void OnRemoveClient(string username)
        {
            RemoveClient?.Invoke(username);
        }

        public async Task HandleClientAsync(CancellationToken ctsToken)
        {
            LogClientConnectionTime();
            NetworkStream stream = Client.GetStream();

            // Assuming you have a method to read the first message
            string initialMessage = await ReadInitialMessage(Client);
            Username = ParseUsername(initialMessage);
            OnAddClient(Username, Client);
            PrintClientInfo();

            byte[] buffer = new byte[3]; // Adjusted to 3 bytes for the "IMG" header

            // Read the header
            await stream.ReadAsync(buffer, 0, buffer.Length);
            string header = Encoding.ASCII.GetString(buffer);

            ClientStatus res = 0;

            if (header == CommandTypes.MSG)
            {
                res = await MessageHandler.HandleMessageAsync(stream, ctsToken, Username);
            }
            else if (header == CommandTypes.IMG)
            {
                await ImageHandler.HandleImageAsync(stream, ImgDirPath, ctsToken);
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

            if (initialMessage.StartsWith(CommandTypes.MSG))
            {
                return initialMessage.Substring(CommandTypes.MSG.Length);
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
            OnRemoveClient(Username);
            Client.Close();
            Console.WriteLine($"{Username} has disconnected...");
        }

        private void CloseClientIfDisconnected(ClientStatus status)
        {
            if (status == ClientStatus.DISCONNECTED)
            {
                CloseClient();
            }
        }

        private void PrintClientInfo()
        {
            if (Client.Connected)
            {
                Console.WriteLine("Client:");
                Console.WriteLine("  Local Endpoint: " + Client.Client.LocalEndPoint);
                Console.WriteLine("  Remote Endpoint: " + Client.Client.RemoteEndPoint);
                Console.WriteLine("  Username: " + Username);
            }
            else
            {
                Console.WriteLine("Client not connected.");
            }
        }
    }
}
