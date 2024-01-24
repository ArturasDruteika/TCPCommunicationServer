 using System.Net.Sockets;
using System.Text;
using MultipleClientServer.Containers;


namespace MultipleClientServer.MessageHandlers.MessageReceivers
{
    public class MessageReceiver : IMessageReceiver
    {
        private int BytesForMsgLen = 16;

        public delegate void MessageBroadcaster(string msg, string username);
        public event MessageBroadcaster NewMessage;

        public MessageReceiver()
        {

        }

        public void OnNewMsg(string msg, string username)
        {
            NewMessage?.Invoke(msg, username);
        }

        public ClientStatus ReceiveMsg(NetworkStream stream)
        {
            try
            {
                int msgLen = GetMsgLen(stream);
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string receivedData = ProcessReceivedData(buffer, bytesRead);
                    //break;
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

        private int GetMsgLen(NetworkStream stream)
        {
            byte[] buffer = new byte[BytesForMsgLen];
            int bytesRead;
            string receivedData = "";

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            return 0;
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
