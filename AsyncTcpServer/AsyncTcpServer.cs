using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    public class AsyncTcpServer
    {
        private TcpListener _listener;
        private bool _isRunning;
        private List<TcpClient> _totalCLientsConnected = new List<TcpClient>();
        private readonly object _clientsLock = new object();

        public AsyncTcpServer(string ipAddress, int port)
        {
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

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using NetworkStream stream = client.GetStream();
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
            finally
            {
                client.Close();
                lock (_clientsLock)
                {
                    _totalCLientsConnected.Remove(client);
                }
            }
        }

        public void Stop()
        {
            PrintClientsInfo();
            _isRunning = false;
            _listener.Stop();
            Console.WriteLine("Server stopped.");
        }

        private async void AcceptClientsAsync()
        {
            while (_isRunning)
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
