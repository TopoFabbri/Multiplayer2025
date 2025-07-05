using System;
using Multiplayer.Network.Objects;
using Multiplayer.NetworkFactory;
using UnityEngine;

namespace Objects
{
    public class ServerObjectSpawner : INetworkFactory
    {
        public ObjectM SpawnObject(SpawnableObjectData data)
        {
            Type modelType = Type.GetType(data.ModelType);
            
            if (modelType == null)
            {
                Debug.LogError($"Model type {data.ModelType} not found. Cannot spawn object.");
                return null;
            }
            
            ObjectM modelInstance = (ObjectM)Activator.CreateInstance(modelType);
            modelInstance.Initialize(data.OwnerId, data.Id);

            return modelInstance;
        }

        public void DestroyObject(int id)
        {
        }
    }
}