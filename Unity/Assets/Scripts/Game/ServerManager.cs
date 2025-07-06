using System;
using System.Collections.Generic;
using System.Net;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.NetworkFactory;
using Objects;

namespace Game
{
    public class ServerManager : GameManager
    {
        private ServerObjectSpawner objectSpawner;
        
        protected override void Awake()
        {
            base.Awake();

            networkManager = new AuthoritativeServer();
            
            int port = int.Parse(Environment.GetCommandLineArgs()[0]);

            ((AuthoritativeServer)networkManager).Init(port);
        }
        
        private void Start()
        {
            objectSpawner = new ServerObjectSpawner();
            
            gameModel = new GameModel(objectSpawner);
            
            GameStateController.State = GameState.InGame;
        }

        protected override void OnHandleSpawnRequest(byte[] data, IPEndPoint ip)
        {
            SpawnRequest message = new NetSpawnable(data).Deserialized();

            List<SpawnableObjectData> spawnables = ((ServerNetManager)networkManager).SpawnObjects(message.spawnableObjects);
            
            gameModel.SpawnObjects(spawnables);
        }
    }
}