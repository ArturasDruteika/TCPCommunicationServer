using MultipleClientServer.Containers;
using MultipleClientServer.ImageHandlers;
using MultipleClientServer.MessageHandlers.MessageReceivers;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace MultipleClientServer.MessageReceivers.MessageListeners
{
    public class MessageListener
    {
        NetworkStream Stream;
        string Username;
        private readonly CancellationTokenSource Cts = new CancellationTokenSource();
        private Task? ListeningTask;
        private readonly IMessageReceiver MessageReceiver;
        private readonly IImageHandler ImageHandler;
        private string ImgDirPath;

        public MessageListener(NetworkStream stream, string username, IMessageReceiver messageReceiver, IImageHandler imageHandler, string imgDirPath)
        {
            Stream = stream;
            Username = username;
            MessageReceiver = messageReceiver;
            ImageHandler = imageHandler;
            ImgDirPath = imgDirPath;
        }

        public void StartListening()
        {
            ListeningTask = Task.Run(ListenForMessages, Cts.Token);
        }

        private void ListenForMessages()
        {
            ClientStatus res = 0;
            byte[] headerBfr = new byte[3];

            while (true)
            {
                Stream.Read(headerBfr, 0, headerBfr.Length);
                string header = Encoding.ASCII.GetString(headerBfr);

                if (header == CommandTypes.MSG)
                {
                    res = MessageReceiver.ReceiveMsg(Stream, Username);
                }
                else if (header == CommandTypes.IMG)
                {
                    //await ImageHandler.HandleImageAsync(stream, ImgDirPath, ctsToken);
                    int a = 0;
                }
                else
                {
                    Console.WriteLine("Client didn't use the protocol...");
                }

                // To drop a load from the while loop
                Thread.Sleep(200);
            }
        }
    }
}
