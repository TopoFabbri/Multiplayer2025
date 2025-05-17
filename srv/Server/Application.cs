using Multiplayer.Network;

namespace Server;

public class Application
{
    private bool running;
    private readonly ServerNetManager networkManager;
    
    public Application()
    {
        Multiplayer.Utils.Timer.Start();
        running = false;

        networkManager = new ServerNetManager();
    }

    public void Run(int port)
    {
        running = true;

        networkManager.Init(port);
        
        while (running)
            networkManager.Update();

        running = false;
    }
}