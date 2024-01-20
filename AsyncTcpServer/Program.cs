using MultipleClientServer.ServerRunners;

class Program
{
    static void Main(string[] args)
    {
        ServerRunner serverRunner = new ServerRunner();
        serverRunner.Run();
    }
}
