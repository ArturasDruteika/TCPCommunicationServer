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
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = Stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string receivedData = ProcessReceivedData(buffer, bytesRead);
                    Console.WriteLine($"{Username}: " + receivedData);
                    OnNewMsg(receivedData, username);
                }

                if (bytesRead == 0)
                {
                    return ClientStatus.DISCONNECTED;
                }

                return ClientStatus.CONNECTED;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return ClientStatus.DISCONNECTED;
            }

            private static KeyValuePair<string, string> ProcessReceivedData(byte[] buffer, int bytesRead)
            {
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                if (receivedData.Length >= 3 && receivedData.Substring(0, 3) == CommandTypes.MSG)
                {
                    receivedData = RemoveHeaderSubstr(receivedData);
                    string sender = GetSenderName(receivedData);
                    string message = RemoveSenderName(receivedData);
                    KeyValuePair<string, string> processedData = new KeyValuePair<string, string>(sender, message);
                    return processedData;
                }
                else
                {
                    return new KeyValuePair<string, string>();
                }

            }
        }
}
