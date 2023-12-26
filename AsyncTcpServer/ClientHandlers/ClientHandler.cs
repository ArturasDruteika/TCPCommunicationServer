using AsyncTcpServer.ImageHandlers;
using AsyncTcpServer.MessageHandlers;
using System.Net.Sockets;
using System.Text;


namespace AsyncTcpServer.ClientHandlers
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly IMessageHandler _messageHandler;
        private readonly IImageHandler _imageHandler;
        private string _imgDirPath;

        public ClientHandler(TcpClient client, IMessageHandler messageHandler, IImageHandler imageHandler, string imgDirPath)
        {
            _client = client;
            _messageHandler = messageHandler;
            _imageHandler = imageHandler;
            _imgDirPath = imgDirPath;
        }

        public async Task HandleClientAsync()
        {
            NetworkStream stream = _client.GetStream();
            byte[] buffer = new byte[3]; // Adjusted to 3 bytes for the "IMG" header

            // Read the header
            await stream.ReadAsync(buffer, 0, buffer.Length);
            string header = Encoding.ASCII.GetString(buffer);

            if (header == CommandTypes.MSG)
            {
                await _messageHandler.HandleMessageAsync(stream);
            }
            else if (header == CommandTypes.IMG)
            {
                await _imageHandler.HandleImageAsync(stream, _imgDirPath);
            }

            _client.Close();
        }
    }

}
