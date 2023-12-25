using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class AsyncTcpServer
{
    private TcpListener _listener;
    private bool _isRunning;

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

    private async void AcceptClientsAsync()
    {
        while (_isRunning)
        {
            TcpClient client = await _listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");
            // Handle the client in a new task
            _ = Task.Run(() => HandleClientAsync(client));
        }
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

                // Echo the data back to the client
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
        }
    }

    public void Stop()
    {
        _isRunning = false;
        _listener.Stop();
        Console.WriteLine("Server stopped.");
    }
}
