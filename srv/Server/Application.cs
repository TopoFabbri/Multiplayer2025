using Multiplayer.Network;

namespace Server;

public class Application
{
    private bool running;
    private readonly ServerNetManager networkManager;
    private int port;
    
    public Application()
    {
        Multiplayer.Utils.Timer.Start();
        running = false;

        networkManager = new ServerNetManager();
    }

    public void Run()
    {
        running = true;

        port = PromptValidPort();

        networkManager.Init(port);
        
        while (running)
            networkManager.Update();

        running = false;
    }

    private static int PromptValidPort()
    {
        return 65432;
        
        int port = 0;

        Console.Write("Enter port number: ");

        string input = Console.ReadLine();
        if (input != null) port = int.Parse(input);

        while (port is < 1024 or > 65535)
        {
            Console.WriteLine("Invalid port number!");
            Console.Write("Enter port number: ");
            
            input = Console.ReadLine();
            if (input != null) port = int.Parse(input);
        }

        return port;
    }
}