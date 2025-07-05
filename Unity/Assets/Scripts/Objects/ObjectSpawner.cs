using System;
using System.Collections.Generic;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Objects;
using Multiplayer.NetworkFactory;
using UnityEngine;

namespace Objects
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private List<ObjectV> prefabs = new();

        private readonly Dictionary<int, ObjectV> viewInstances = new();

        public ObjectM SpawnObject(SpawnableObjectData data)
        {
            if (data.PrefabId < 0 || data.PrefabId >= prefabs.Count)
            {
                Debug.LogError($"PrefabId {data.PrefabId} is out of bounds for prefabs or model types list. Cannot spawn object.");
                return null;
            }

            if (viewInstances.ContainsKey(data.Id))
            {
                Debug.LogWarning($"An object with ID {data.Id} already exists. Ignoring duplicate spawn request.");
                return null;
            }

            ObjectM modelInstance = (ObjectM)Activator.CreateInstance(prefabs[data.PrefabId].ModelType);
            modelInstance.Initialize(data.OwnerId, data.Id);

            ObjectV viewInstance = Instantiate(prefabs[data.PrefabId]);
            viewInstance.name = data.Id + "_" + prefabs[data.PrefabId].name;
            viewInstance.Initialize(modelInstance);

            viewInstances.Add(data.Id, viewInstance);

            return modelInstance;
        }

        public void RequestSpawn(List<SpawnableObjectData> spawnablesData)
        {
            foreach (SpawnableObjectData spawnableData in spawnablesData)
            {
                if (spawnableData.PrefabId >= 0 && spawnableData.PrefabId < prefabs.Count) continue;

                Debug.LogWarning(spawnableData.PrefabId + " is not a valid object number.");
                return;
            }

            SpawnRequest spawnRequest = new(spawnablesData);

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