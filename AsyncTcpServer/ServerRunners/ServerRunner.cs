using MultipleClientServer.ImageHandlers;
using MultipleClientServer.MessageHandlers.MessageReceivers;
using MultipleClientServer.MessageHandlers.MessageSenders;
using CustomServer;

namespace MultipleClientServer.ServerRunners
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
            MessageReceiver msgReceiver = new MessageReceiver();
            ImageHandler imgReceiver = new ImageHandler();
            MessageSender msgSender = new MessageSender();

            Server server = new Server(
                IP,
                PORT,
                msgReceiver,
                imgReceiver,
                msgSender
                );

            server.SubscribeToMessageBroadcaster(msgReceiver);

            server.Start();

            Console.WriteLine("Press Enter to stop the server...");
            Console.ReadLine();

            server.Stop();
        }

    }
}
