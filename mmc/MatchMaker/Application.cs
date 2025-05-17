using Multiplayer.Network;
using Timer = Multiplayer.Utils.Timer;

namespace MatchMaker;

public class Application
{
    private readonly NetworkManager networkManager = new MatchMakerNetManager();
    private bool running = false;
    private const int Port = 65432;

    public Application()
    {
        Timer.Start();
    }
    
    public void Run()
    {
        running = true;
        
        networkManager.Init(Port);
        
        while (running)
            networkManager.Update();

        running = false;
    }
}