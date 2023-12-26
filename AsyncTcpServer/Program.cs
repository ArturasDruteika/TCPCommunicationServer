using AsyncTcpServer.ImageHandlers;
using AsyncTcpServer.MessageHandlers;
using CustomServer;


class Program
{

    const string IP = "127.0.0.1";
    const int PORT = 1024;

    static void Main(string[] args)
    {
        MessageHandler msgHandler = new MessageHandler();
        ImageHandler imgHandler = new ImageHandler();

        AsyncTCPServer server = new AsyncTCPServer(IP, PORT, msgHandler, imgHandler);
        server.Start();

        Console.WriteLine("Press Enter to stop the server...");
        Console.ReadLine();

        server.Stop();
    }
}
