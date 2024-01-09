using AsyncTcpServer.ImageHandlers;
using AsyncTcpServer.MessageHandlers.MessageReceivers;
using Client.MessageHandlers.MessageSenders;
using CustomServer;

namespace AsyncTcpServer.ServerRunners
{
    public class ServerRunner
    {
        const string IP = "127.0.0.1";
        const int PORT = 1024;

        public ServerRunner()
        {

        }

        public void Run() 
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

            msgHandler.Attach(server);

            server.Start();

            Console.WriteLine("Press Enter to stop the server...");
            Console.ReadLine();

            server.Stop();
        }

    }
}
