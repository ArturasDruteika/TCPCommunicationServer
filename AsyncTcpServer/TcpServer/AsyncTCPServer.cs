using AsyncTcpServer.ClientHandlers;
using AsyncTcpServer.ImageHandlers;
using AsyncTcpServer.MessageHandlers.MessageReceivers;
using AsyncTcpServer.Observer;
using AsyncTcpServer.Utils;
using Client.MessageHandlers.MessageSenders;
using System.Net;
using System.Net.Sockets;


namespace CustomServer
{
    public class AsyncTCPServer : ISubscriber
    {
        // 
        private TcpListener Listener;
        private bool IsRunning = false;
        private string ImgDirPath = string.Empty;
        private CancellationTokenSource Cts = new CancellationTokenSource();
        private readonly IMessageReceiver MessageHandler;
        private readonly IImageHandler ImageHandler;
        private readonly IMessageSender MessageSender;
        private Dictionary<string, TcpClient> ClientDict = new Dictionary<string, TcpClient>();

        public AsyncTCPServer(
            string ipAddress,
            int port,
            IMessageReceiver messageHandler,
            IImageHandler imageHandler,
            IMessageSender messageSender
            )
        {
            Listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            MessageHandler = messageHandler;
            ImageHandler = imageHandler;
            MessageSender = messageSender;
            ImgDirPath = ImageSavePathManager.GetImageSavePath();
        }

        public void AddClient(string username, TcpClient client)
        {
            ClientDict.Add(username, client);
        }

        public void RemoveClient(string username) 
        {
            ClientDict.Remove(username);
        }

        public void BroadcastToOthers(string msg, string username)
        {
            foreach (KeyValuePair<string, TcpClient> client in ClientDict)
            {
                if (client.Key != username)
                {
                    NetworkStream stream = client.Value.GetStream();
                    MessageSender.SendMsg(msg, stream, Cts.Token);
                }
            }
        }

        public void Start()
        {
            Listener.Start();
            IsRunning = true;
            Console.WriteLine("Server started. Listening for connections...");
            AcceptClientsAsync();
        }

        public void Stop()
        {
            IsRunning = false;
            Cts.Cancel();
            Listener.Stop();
            Console.WriteLine("Server stopped.");
        }

        private async void AcceptClientsAsync()
        {
            try
            {
                while (!Cts.Token.IsCancellationRequested)
                {
                    if (Listener.Pending())
                    {
                        TcpClient client = await Listener.AcceptTcpClientAsync(Cts.Token);
                        ClientHandler clientHandler = new ClientHandler(client, MessageHandler, ImageHandler, ImgDirPath);
                        clientHandler.Attach(this);
                        Console.WriteLine("Client connected.");
                        _ = Task.Run(() => clientHandler.HandleClientAsync(Cts.Token));
                    }
                    else
                    {
                        await Task.Delay(100); // Wait a bit before checking again
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Listener has been stopped
                Console.WriteLine("Listener has been stopped.");
            }
        }
    }
}
