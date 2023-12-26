using CustomServer;


class Program
{

    const string IP = "127.0.0.1";
    const int PORT = 1024;

    static void Main(string[] args)
    {
        AsyncTCPServer server = new AsyncTCPServer(IP, PORT);
        server.Start();

        Console.WriteLine("Press Enter to stop the server...");
        Console.ReadLine();

        server.Stop();
    }
}
