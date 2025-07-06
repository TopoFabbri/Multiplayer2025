using System.Collections.Generic;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Objects;
using Multiplayer.NetworkFactory;
using Multiplayer.Reflection;
using Multiplayer.Utils;
using Objects;

namespace Game
{
    public class GameModel : Model
    {
        private readonly INetworkFactory objectSpawner;
        private readonly Board board;
        private const int PawnQty = 15;
        
        [Sync] private readonly Dictionary<int, ObjectM> objects = new();

        public GameModel(INetworkFactory objectSpawner)
        {
            this.objectSpawner = objectSpawner;
            board = new Board();

            GameStateController.StateChanged += OnStateChanged;
        }

        ~GameModel()
        {
            GameStateController.StateChanged -= OnStateChanged;
        }
        
        public void SpawnObjects(List<SpawnableObjectData> spawnables)
        {
            foreach (SpawnableObjectData spawnableObject in spawnables)
            {
                if (objects.ContainsKey(spawnableObject.Id))
                    continue;
                
                ObjectM model =  objectSpawner.SpawnObject(spawnableObject);

                if (model == null) continue;
                
                objects.Add(model.ObjectId, model);
                
                if (spawnableObject.OwnerId != NetworkManager.Instance.Id && NetworkManager.Instance.Id != 0) continue;

                board.PlaceObject(model as BoardPiece);
            }
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.InGame)
                OnConnect();
            else
                OnDisconnect();
        }

        private void OnConnect()
        {
            board.CreateBoard(30, 30);

            List<SpawnableObjectData> spawnablesData = new() { new SpawnableObjectData { OwnerId = NetworkManager.Instance.Id, PrefabId = 0, ModelType = typeof(TowerM).FullName } };

            for (int i = 0; i < PawnQty; i++)
                spawnablesData.Add(new SpawnableObjectData { OwnerId = NetworkManager.Instance.Id, PrefabId = 1, ModelType = typeof(PawnM).FullName});
            
            SpawnRequest spawnRequest = new(spawnablesData);

            NetworkManager.Instance.SendData(new NetSpawnable(spawnRequest).Serialize());
        }

        private void OnDisconnect()
        {
            foreach (int key in objects.Keys)
                objectSpawner.DestroyObject(key);
            
            objects.Clear();
        }
    }
}