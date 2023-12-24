using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Server
{
    public class TCPServer
    {
        private TcpListener _tcpListener;
        private bool _isRunning;

        public TCPServer(string ipAddress, int port)
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            _tcpListener = new TcpListener(ip, port);
            _isRunning = false;
        }

        public void Run()
        {
            _tcpListener.Start();
            _isRunning = true;
            Console.WriteLine("Server started. Waiting for connections...");

            while (_isRunning)
            {
                try
                {
                    TcpClient client = _tcpListener.AcceptTcpClient();
                    Task.Run(() => HandleClient(client)); // Handle each client in a separate task
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("SocketException: " + ex.Message);
                    Stop();
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine("Received: " + receivedData);

                        string msg = "Welcome Connected to the Server!!!";
                        // Echo the data back to the client
                        byte[] sendData = Encoding.UTF8.GetBytes(msg);
                        stream.Write(sendData, 0, sendData.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _tcpListener.Stop();
            Console.WriteLine("Server stopped.");
        }
    }
}
