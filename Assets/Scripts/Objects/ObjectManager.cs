using System.Collections.Generic;
using Network;
using Network.Messages;
using UnityEngine;

namespace Objects
{
    public class ObjectManager : MonoBehaviour
    {
        [SerializeField] private List<SpawnableObject> prefabList = new();

        private readonly Dictionary<int, SpawnableObject> spawnedObjects = new();
        
        private void OnEnable()
        {
            NetworkManager.Instance.OnReceiveDataAction += OnReceiveDataHandler;
        }
        
        private void OnDisable()
        {
            NetworkManager.Instance.OnReceiveDataAction -= OnReceiveDataHandler;
        }

        private void OnReceiveDataHandler(byte[] data)
        {
            if (MessageHandler.GetMessageType(data) == MessageType.Spawnable)
            {
                Spawnable message = new NetSpawnable(data).Deserialized();

                SpawnableObject spawnedObject = Spawn(message.spawnableNumber);

                spawnedObjects.Add(message.id, spawnedObject);
            }
            else if (MessageHandler.GetMessageType(data) == MessageType.Position)
            {
                Position message = new NetVector3(data).Deserialized();

                UpdatePosition(message.objId, message.position);
            }
            
        }

        private SpawnableObject Spawn(int id)
        {
            SpawnableObject instance = prefabList[id] == null ? null : prefabList[id].Spawn();

            if (instance != null)
                instance.transform.parent = transform;
            
            return instance;
        }

        private void UpdatePosition(int id, Vector3 position)
        {
            if (spawnedObjects.TryGetValue(id, out SpawnableObject spawnedObject))
                spawnedObject.transform.position = position;
        }
    }
}
