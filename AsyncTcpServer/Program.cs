using AsyncTcpServer.ImageHandlers;
using AsyncTcpServer.MessageHandlers.MessageReceivers;
using Client.MessageHandlers.MessageSenders;
using CustomServer;


class Program
{

    const string IP = "127.0.0.1";
    const int PORT = 1024;

    static void Main(string[] args)
    {
        MessageReceiver msgHandler = new MessageReceiver();
        ImageHandler imgHandler = new ImageHandler();
        MessageSender msgSender = new MessageSender();

        AsyncTCPServer server = new AsyncTCPServer(
            IP, 
            PORT, 
            msgHandler, 
            imgHandler, 
            msgSender
            );

        msgHandler.Attach( server );

        server.Start();

        Console.WriteLine("Press Enter to stop the server...");
        Console.ReadLine();

        server.Stop();
    }
}
