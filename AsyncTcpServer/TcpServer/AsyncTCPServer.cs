using AsyncTcpServer.ClientHandlers;
using AsyncTcpServer.ImageHandlers;
using AsyncTcpServer.MessageHandlers;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace CustomServer
{
    public class AsyncTCPServer
    {
        private IPAddress? _ip; // Field declaration
        private TcpListener _listener;
        private bool _isRunning = false;
        private string _imageSavePath = "";
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly object _clientsLock = new object();
        private MessageHandler _messageHandler = new MessageHandler();
        private ImageHandler _imageHandler = new ImageHandler();

        public AsyncTCPServer(string ipAddress, int port)
        {
            SetUpImgSavePath(); // Ensure this method doesn't use _ip before it's initialized
            _ip = IPAddress.Parse(ipAddress); // Initializing _ip
            _listener = new TcpListener(_ip, port);
        }

        public void Start()
        {
            _listener.Start();
            _isRunning = true;
            Console.WriteLine("Server started. Listening for connections...");
            AcceptClientsAsync();
        }

        public void Stop()
        {
            //PrintClientsInfo();
            _isRunning = false;
            _cts.Cancel();
            _listener.Stop();
            Console.WriteLine("Server stopped.");
        }

        private async void AcceptClientsAsync()
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (_listener.Pending())
                    {
                        TcpClient client = await _listener.AcceptTcpClientAsync();
                        ClientHandler _clientHandler = new ClientHandler(client, _messageHandler, _imageHandler, _imageSavePath);
                        Console.WriteLine("Client connected.");
                        lock (_clientsLock)
                        {
                            PrintClientInfo(client);
                        }
                        _ = Task.Run(() => _clientHandler.HandleClientAsync());
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

        private void SetUpImgSavePath()
        {
            string executablePath = Assembly.GetExecutingAssembly().Location;
            string executableDirectory = Path.GetDirectoryName(executablePath);
            _imageSavePath = Path.Combine(executableDirectory, "data", "images");
            Directory.CreateDirectory(_imageSavePath);
        }

        private void PrintClientInfo(TcpClient client)
        {
            lock (_clientsLock)
            {
                if (client.Connected)
                {
                    Console.WriteLine("Client:");
                    Console.WriteLine("  Local Endpoint: " + client.Client.LocalEndPoint);
                    Console.WriteLine("  Remote Endpoint: " + client.Client.RemoteEndPoint);
                }
                else
                {
                    Console.WriteLine("Client not connected.");
                }
            }
        }
    }
}
