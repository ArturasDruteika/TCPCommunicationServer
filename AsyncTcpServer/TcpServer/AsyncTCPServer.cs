using AsyncTcpServer.ClientHandlers;
using AsyncTcpServer.ImageHandlers;
using AsyncTcpServer.MessageHandlers.MessageReceivers;
using AsyncTcpServer.Utils;
using Client.MessageHandlers.MessageSenders;
using System.Net;
using System.Net.Sockets;


namespace CustomServer
{
    public class AsyncTCPServer
    {
        private TcpListener Listener;
        private bool IsRunning = false;
        private string ImgDirPath = "";
        private CancellationTokenSource Cts = new CancellationTokenSource();
        private readonly IMessageHandler MessageHandler;
        private readonly IImageHandler ImageHandler;
        private readonly IMessageSender MessageSender;

        public AsyncTCPServer(
            string ipAddress, 
            int port, 
            IMessageHandler messageHandler, 
            IImageHandler imageHandler, 
            IMessageSender messageSender
            )
        {
            Listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            MessageHandler = messageHandler;
            ImageHandler = imageHandler;
            MessageSender = messageSender;
            ImgDirPath = ImageSavePathManager.GetImageSavePath();
        }

        public void Start()
        {
            Listener.Start();
            IsRunning = true;
            Console.WriteLine("Server started. Listening for connections...");
            AcceptClientsAsync();
        }

        public void Stop()
        {
            IsRunning = false;
            Cts.Cancel();
            Listener.Stop();
            Console.WriteLine("Server stopped.");
        }

        private async void AcceptClientsAsync()
        {
            try
            {
                while (!Cts.Token.IsCancellationRequested)
                {
                    if (Listener.Pending())
                    {
                        TcpClient client = await Listener.AcceptTcpClientAsync(Cts.Token);
                        ClientHandler clientHandler = new ClientHandler(client, MessageHandler, ImageHandler, ImgDirPath);
                        Console.WriteLine("Client connected.");
                        _ = Task.Run(() => clientHandler.HandleClientAsync(Cts.Token));
                    }
                    else
                    {
                        await Task.Delay(100); // Wait a bit before checking again
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Listener has been stopped
                Console.WriteLine("Listener has been stopped.");
            }
        }

        //private async Task SendMsgToClient(string msg)
        //{
        //    await MessageSender.SendMsg(msg, _stream, );
        //}
    }
}
