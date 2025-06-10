using System.Collections.Generic;
using Multiplayer.Network.Objects;
using Multiplayer.NetworkFactory;
using Multiplayer.Reflection;
using UnityEngine;

namespace Objects
{
    public class ModelObjectManager : MonoBehaviour
    {
        [SerializeField] private List<ObjectV> prefabs = new();
        [Sync] private readonly List<ObjectM> objects = new();
        
        public void SpawnObject(SpawnableObjectData data)
        {
            ObjectM modelInstance = new();
            modelInstance.Initialize(data.OwnerId);
            objects.Add(modelInstance);
            
            ObjectV viewInstance = Instantiate(prefabs[data.PrefabId]);
            
            viewInstance.Initialize(modelInstance);
        }
    }
}