using System.Collections.Generic;
using Game;
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
        
        private readonly Dictionary<int, ObjectV> viewInstances = new();
        
        public ObjectM SpawnObject(SpawnableObjectData data)
        {
            if (data.PrefabId < 0 || data.PrefabId >= prefabs.Count)
            {
                Debug.LogWarning(data.PrefabId + " is not a valid object number.");
                return null;
            }

            ObjectV viewInstance = Instantiate(prefabs[data.PrefabId]);
            ObjectM modelInstance = viewInstance.Initialize(data);
            
            viewInstances.Add(data.Id, viewInstance);

            return modelInstance;
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

        public void ClearViewInstances()
        {
            foreach (KeyValuePair<int, ObjectV> objectV in viewInstances)
                Destroy(objectV.Value.gameObject);
            
            viewInstances.Clear();
        }
    }
}