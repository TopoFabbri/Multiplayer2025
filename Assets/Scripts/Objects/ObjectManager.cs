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
            if (NetworkManager.Instance)
                NetworkManager.Instance.OnReceiveDataAction -= OnReceiveDataHandler;
        }

        private void OnReceiveDataHandler(byte[] data)
        {
            if (MessageHandler.GetMessageType(data) == MessageType.Spawnable)
            {
                List<Spawnable> message = new NetSpawnable(data).Deserialized();

                foreach (Spawnable spawnable in message)
                {
                    if (spawnedObjects.ContainsKey(spawnable.id)) continue;
                    
                    SpawnableObject spawnedObject = Spawn(spawnable.spawnableNumber);
                    spawnedObjects.Add(spawnable.id, spawnedObject);
                }
            }
            else if (MessageHandler.GetMessageType(data) == MessageType.Position)
            {
                Position message = new NetVector3(data).Deserialized();

                UpdatePosition(message.objId, message.position);
            }
            
        }

        private SpawnableObject Spawn(int id)
        {
            SpawnableObject instance = prefabList[id] == null ? null : prefabList[id].Spawn(this);

            if (instance != null)
                instance.transform.parent = transform;
            
            return instance;
        }

        public void UpdatePosition(int id, Vector3 position)
        {
            if (spawnedObjects.TryGetValue(id, out SpawnableObject spawnedObject))
                spawnedObject.transform.position = position;
        }

        public void RequestSpawn(int objNumber)
        {
            if (objNumber < 0 || objNumber >= prefabList.Count)
            {
                Debug.LogWarning(objNumber + " is not a valid object number.");
                return;
            }
            
            Spawnable spawnable = new()
            {
                spawnableNumber = objNumber
            };
            
            List<Spawnable> message = new() { spawnable };

            NetworkManager.Instance.SendData(new NetSpawnable(message).Serialize());
        }
    }
}
