using System.Net.Sockets;
using System.Text;
using AsyncTcpServer.Containers;
using AsyncTcpServer.Observer;


namespace AsyncTcpServer.MessageHandlers.MessageReceivers
{
    public class MessageReceiver : IMessageReceiver
    {
        public delegate void MessageBroadcaster(string msg, string username);
        public event MessageBroadcaster NewMessage;

        public MessageReceiver()
        {

        }

        public void OnNewMsg(string msg, string username)
        {
            NewMessage?.Invoke(msg, username);
        }

        public async Task<ClientStatus> HandleMessageAsync(NetworkStream stream, CancellationToken ctsToken, string username)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ctsToken)) != 0)
                {
                    string receivedData = ProcessReceivedData(buffer, bytesRead);
                    Console.WriteLine($"{username}: " + receivedData);
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
        }

        private static string ProcessReceivedData(byte[] buffer, int bytesRead)
        {
            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            if (receivedData.Length >= 3 && receivedData.Substring(0, 3) == CommandTypes.MSG)
            {
                receivedData = RemoveHeaderSubstr(receivedData);
            }
            return receivedData;
        }

        private static string RemoveHeaderSubstr(string message)
        {
            return message[3..];
        }
    }
}
