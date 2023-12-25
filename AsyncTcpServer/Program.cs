using Server;


class Program
{
    static void Main(string[] args)
    {
        AsyncTcpServer server = new AsyncTcpServer("127.0.0.1", 1024);
        server.Start();

        Console.WriteLine("Press Enter to stop the server...");
        Console.ReadLine();

        server.Stop();
    }
}
