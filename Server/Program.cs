using Server;


class Program
{
    public const string IP = "127.0.0.1";
    public const int PORT = 1024;

    static void Main(string[] args)
    {
        TCPServer tcpServerWrapper = new TCPServer(IP, PORT);
        tcpServerWrapper.Run();
    }
}
