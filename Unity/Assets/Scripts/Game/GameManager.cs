using System.Net;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.Reflection;
using Multiplayer.Utils;
using Utils;

namespace Game
{
    public abstract class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [Sync] protected GameModel gameModel;

        protected NetworkManager networkManager;

        protected override void Awake()
        {
            base.Awake();
            
            Timer.Start();
        }

        private void OnEnable()
        {
            MessageHandler.TryAddHandler(MessageType.SpawnRequest, OnHandleSpawnRequest);
        }

        private void OnDisable()
        {
            MessageHandler.TryRemoveHandler(MessageType.SpawnRequest, OnHandleSpawnRequest);
        }

        protected virtual void OnHandleSpawnRequest(byte[] data, IPEndPoint ip)
        {
            SpawnRequest message = new NetSpawnable(data).Deserialized();
            gameModel.SpawnObjects(message.spawnableObjects);
        }

        private void Update()
        {
            if (networkManager.IsInitiated)
                networkManager?.Update();
        }

        private void LateUpdate()
        {
            if (GameStateController.State == GameState.InGame)
                gameModel?.Update(); 
        }
    }
}