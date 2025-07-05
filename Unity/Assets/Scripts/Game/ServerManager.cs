using Multiplayer.Network;

namespace Game
{
    public class ServerManager : GameManager
    {
        protected override void Awake()
        {
            base.Awake();

            networkManager = new AuthoritativeServer();
        }
        
        private void Start()
        {
            gameModel = new GameModel(objectSpawner);
            
            GameStateController.State = GameState.InGame;
        }
    }
}