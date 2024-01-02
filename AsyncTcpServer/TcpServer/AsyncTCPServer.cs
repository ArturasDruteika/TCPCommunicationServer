using AsyncTcpServer.ClientHandlers;
using AsyncTcpServer.ImageHandlers;
using AsyncTcpServer.MessageHandlers;
using AsyncTcpServer.Utils;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace CustomServer
{
    public class AsyncTCPServer
    {
        private TcpListener _listener;
        private bool _isRunning = false;
        private string _imgDirPath = "";
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly IMessageHandler _messageHandler;
        private readonly IImageHandler _imageHandler;

        public AsyncTCPServer(string ipAddress, int port, IMessageHandler messageHandler, IImageHandler imageHandler)
        {
            _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _messageHandler = messageHandler;
            _imageHandler = imageHandler;
            _imgDirPath = ImageSavePathManager.GetImageSavePath();
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
                        TcpClient client = await _listener.AcceptTcpClientAsync(_cts.Token);
                        ClientHandler clientHandler = new ClientHandler(client, _messageHandler, _imageHandler, _imgDirPath);
                        Console.WriteLine("Client connected.");
                        _ = Task.Run(() => clientHandler.HandleClientAsync(_cts.Token));
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
