using CustomServer;


class Program
{

    const string IP = "127.0.0.1";
    const int PORT = 1024;

    static void Main(string[] args)
    {
        AsyncTcpServer server = new AsyncTcpServer(IP, PORT);
        server.Start();

        Console.WriteLine("Press Enter to stop the server...");
        Console.ReadLine();

        server.Stop();
    }
}
