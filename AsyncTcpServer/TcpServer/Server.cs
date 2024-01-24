using MultipleClientServer.ClientHandlers;
using MultipleClientServer.ImageHandlers;
using MultipleClientServer.MessageHandlers.MessageReceivers;
using MultipleClientServer.Utils;
using MultipleClientServer.MessageHandlers.MessageSenders;
using System.Net;
using System.Net.Sockets;


namespace CustomServer
{
    public class Server
    {
        // 
        private TcpListener Listener;
        private bool IsRunning = false;
        private string ImgDirPath = string.Empty;
        private CancellationTokenSource Cts = new CancellationTokenSource();
        private readonly IMessageReceiver MessageReceiver;
        private readonly IImageHandler ImageReceiver;
        private readonly IMessageSender MessageSender;
        private Dictionary<string, TcpClient> ClientsDict = new Dictionary<string, TcpClient>();

        public Server(
            string ipAddress,
            int port,
            IMessageReceiver messageReceiver,
            IImageHandler imageReceiver,
            IMessageSender messageSender
            )
        {
            Listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            MessageReceiver = messageReceiver;
            ImageReceiver = imageReceiver;
            MessageSender = messageSender;
            ImgDirPath = ImageSavePathManager.GetImageSavePath();
        }

        public void SubscribeToMessageBroadcaster(MessageReceiver messageReceiver)
        {
            messageReceiver.NewMessage += BroadcastToOthers;
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

        private void SubscribeToClientReceiver(ClientHandler clientReceiver)
        {
            clientReceiver.NewClient += AddClient;
            clientReceiver.RemoveClient += RemoveClient;
        }

        private void AddClient(string username, TcpClient client)
        {
            ClientsDict.Add(username, client);
        }

        private void RemoveClient(string username) 
        {
            ClientsDict.Remove(username);
        }

        private void BroadcastToOthers(string msg, string username)
        {
            foreach (KeyValuePair<string, TcpClient> client in ClientsDict)
            {
                if (client.Key != username)
                {
                    NetworkStream stream = client.Value.GetStream();
                    MessageSender.SendMsg(msg, username, stream, Cts.Token);
                }
            }
        }

        // Move this functionality to a separate class
        private async void AcceptClientsAsync()
        {
            try
            {
                while (!Cts.Token.IsCancellationRequested)
                {
                    if (Listener.Pending())
                    {
                        TcpClient client = await Listener.AcceptTcpClientAsync(Cts.Token);
                        ClientHandler clientReceiver = new ClientHandler(client, MessageReceiver, ImageReceiver, ImgDirPath);
                        SubscribeToClientReceiver(clientReceiver);
                        Console.WriteLine("Client connected.");
                        _ = Task.Run(() => clientReceiver.HandleClientAsync(Cts.Token));
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
