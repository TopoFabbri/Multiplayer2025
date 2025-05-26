using Multiplayer.Network;
using Multiplayer.Utils;

namespace Server;

public class Application
{
    private readonly ServerNetManager networkManager;
    
    public Application()
    {
        Multiplayer.Utils.Timer.Start();

        networkManager = new ServerNetManager();
    }

    public void Run(int port)
    {
        Console.Title = "Server at port " + port;
        
        try
        {
            networkManager.Init(port);
        
            while (networkManager.Active)
                networkManager.Update();

            Log.NewLine();
            Log.Write("Server at port " + port + " closed.");
        }
        finally
        {
            if (networkManager.Active)
                Console.ReadKey();
            else
                Thread.Sleep(3000);
        }
    }
}