using System.Collections.Generic;
using System.Net;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Objects;
using Multiplayer.NetworkFactory;
using Multiplayer.Reflection;
using Objects;

namespace Game
{
    public class GameModel : Model
    {
        private readonly ModelObjectManager objectSpawner;
        
        [Sync] private readonly Dictionary<int, ObjectM> objects = new();

        public GameModel(ModelObjectManager objectSpawner)
        {
            this.objectSpawner = objectSpawner;

            MessageHandler.TryAddHandler(MessageType.SpawnRequest, OnHandleSpawnRequest);
            GameStateController.StateChanged += OnStateChanged;
        }

        ~GameModel()
        {
            MessageHandler.TryRemoveHandler(MessageType.SpawnRequest, OnHandleSpawnRequest);
            GameStateController.StateChanged -= OnStateChanged;
        }

        public void RequestSpawn(SpawnableObjectData spawnableData)
        {
            objectSpawner.RequestSpawn(spawnableData);
        }
        
        private void OnHandleSpawnRequest(byte[] data, IPEndPoint ip)
        {
            SpawnRequest message = new NetSpawnable(data).Deserialized();

            foreach (SpawnableObjectData spawnableObject in message.spawnableObjects)
            {
                if (objects.ContainsKey(spawnableObject.Id))
                    continue;
                
                ObjectM model = objectSpawner.SpawnObject(spawnableObject);

                if (model != null)
                    objects.Add(model.ObjectId, model);
            }
        }

        private void OnStateChanged(GameState state)
        {
            if (state != GameState.InGame)
                OnDisconnect();
        }

        private void OnDisconnect()
        {
            objects.Clear();
            objectSpawner.ClearViewInstances();
        }
    }
}