using System.Collections.Generic;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Objects;
using Multiplayer.NetworkFactory;
using Multiplayer.Reflection;
using UnityEngine;

namespace Objects
{
    public class ModelObjectManager : MonoBehaviour
    {
        [SerializeField] private List<ObjectV> prefabs = new();
        [Sync] private readonly Dictionary<int, ObjectM> objects = new();

        public ModelObjectManager()
        {
            MessageHandler.TryAddHandler(MessageType.SpawnRequest, OnHandleSpawnRequest);
        }

        ~ModelObjectManager()
        {
            MessageHandler.TryRemoveHandler(MessageType.SpawnRequest, OnHandleSpawnRequest);
        }
        
        private void OnHandleSpawnRequest(byte[] data, System.Net.IPEndPoint ip)
        {
            SpawnRequest message = new NetSpawnable(data).Deserialized();

            foreach (SpawnableObjectData spawnableObject in message.spawnableObjects)
            {
                if (spawnableObject.PrefabId < 0 || spawnableObject.PrefabId >= prefabs.Count)
                {
                    Debug.LogWarning(spawnableObject.PrefabId + " is not a valid object number.");
                    continue;
                }
                
                SpawnObject(spawnableObject);
            }
        }
        
        public void SpawnObject(SpawnableObjectData data)
        {
            if (objects.ContainsKey(data.Id))
                return;
            
            ObjectM modelInstance = new();
            modelInstance.Initialize(data.OwnerId, data.Id);
            objects.Add(data.Id, modelInstance);
            
            ObjectV viewInstance = Instantiate(prefabs[data.PrefabId]);
            
            viewInstance.Initialize(modelInstance);
        }
        
        public void RequestSpawn(SpawnableObjectData spawnableData)
        {
            if (spawnableData.PrefabId < 0 || spawnableData.PrefabId >= prefabs.Count)
            {
                Debug.LogWarning(spawnableData.PrefabId + " is not a valid object number.");
                return;
            }

            SpawnRequest spawnRequest = new(new List<SpawnableObjectData> { spawnableData });

            NetworkManager.Instance.SendData(new NetSpawnable(spawnRequest).Serialize());
        }
    }
}