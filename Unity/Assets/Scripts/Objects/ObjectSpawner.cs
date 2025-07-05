using System;
using System.Collections.Generic;
using Multiplayer.Network.Objects;
using Multiplayer.NetworkFactory;
using UnityEngine;

namespace Objects
{
    public class ObjectSpawner : MonoBehaviour, INetworkFactory
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

            Type modelType = Type.GetType(data.ModelType);
            
            if (modelType == null)
            {
                Debug.LogError($"Model type {data.ModelType} not found. Cannot spawn object.");
                return null;
            }
            
            ObjectM modelInstance = (ObjectM)Activator.CreateInstance(modelType);
            modelInstance.Initialize(data.OwnerId, data.Id);

            ObjectV viewInstance = Instantiate(prefabs[data.PrefabId]);
            viewInstance.name = data.Id + "_" + prefabs[data.PrefabId].name;
            viewInstance.Initialize(modelInstance);

            viewInstances.Add(data.Id, viewInstance);

            return modelInstance;
        }

        public void DestroyObject(int id)
        {
            if (!viewInstances.TryGetValue(id, out ObjectV instance))
                return;

            Destroy(instance.gameObject);
            viewInstances.Remove(id);
        }
    }
}