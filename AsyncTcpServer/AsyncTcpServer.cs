using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace CustomServer
{
    public class AsyncTcpServer
    {
        private readonly string _ip = "";
        private readonly int _port = 0;
        private TcpListener _listener;
        private bool _isRunning = false;
        private List<TcpClient> _totalCLientsConnected = new List<TcpClient>();
        private readonly object _clientsLock = new object();
        private int _receivedFileCount = 0;
        private readonly string _imageSavePath = "";
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public AsyncTcpServer(string ipAddress, int port)
        {
            _ip = ipAddress;
            _port = port;

            string executablePath = Assembly.GetExecutingAssembly().Location;
            string executableDirectory = Path.GetDirectoryName(executablePath);
            _imageSavePath = Path.Combine(executableDirectory, "data", "images");
            Directory.CreateDirectory(_imageSavePath);

            IPAddress ip = IPAddress.Parse(ipAddress);
            _listener = new TcpListener(ip, port);
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
            PrintClientsInfo();
            _isRunning = false;
            _cts.Cancel();
            _listener.Stop();
            Console.WriteLine("Server stopped.");
        }

        private async Task HandleStrMsgAsync(NetworkStream stream)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received: " + receivedData);

                    byte[] sendData = Encoding.UTF8.GetBytes(receivedData);
                    await stream.WriteAsync(sendData, 0, sendData.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        public async Task HandleImgMsgAsync(NetworkStream stream)
        {
            // Specify the path and name for the saved image
            string imagePath = _imageSavePath + "/received_image.jpg";
            using var fileStream = new FileStream(imagePath, FileMode.Create);

            // Read the image data from the stream and write it to the file
            await stream.CopyToAsync(fileStream);

            Console.WriteLine($"Image saved to {imagePath}");
            // Further processing of the image...
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[3]; // Adjusted to 3 bytes for the "IMG" header

            // Read the header
            await stream.ReadAsync(buffer, 0, buffer.Length);
            string header = Encoding.ASCII.GetString(buffer);

            if (header == CommandTypes.MSG)
            {
                await HandleStrMsgAsync(stream);
            }
            else if (header == CommandTypes.IMG)
            {
                await HandleImgMsgAsync(stream);
            }

            client.Close();
            _totalCLientsConnected.Remove(client);
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
                        Console.WriteLine("Client connected.");
                        lock (_clientsLock)
                        {
                            _totalCLientsConnected.Add(client);
                            PrintClientInfo(client);
                        }
                        _ = Task.Run(() => HandleClientAsync(client));
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

        private void PrintClientsInfo()
        {
            lock (_clientsLock)
            {
                if (_totalCLientsConnected.Count == 0)
                {
                    Console.WriteLine("All clients have disconnected.");
                }
                else
                {
                    foreach (TcpClient client in _totalCLientsConnected)
                    {
                        PrintClientInfo(client);
                    }
                }
            }
        }
    }
}
