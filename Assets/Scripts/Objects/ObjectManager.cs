using System.Collections.Generic;
using System.Linq;
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
            if (MessageHandler.GetMessageType(data) == MessageType.SpawnRequest)
            {
                List<SpawnRequest> message = new NetSpawnable(data).Deserialized();

                if (spawnedObjects.Count <= 0)
                    Player.PlayerID = message.Last().id;
                
                foreach (SpawnRequest spawnable in message)
                {
                    if (spawnedObjects.ContainsKey(spawnable.id)) continue;
                    
                    SpawnableObject spawnedObject = Spawn(spawnable.spawnableNumber, spawnable.id);
                    spawnedObjects.Add(spawnable.id, spawnedObject);
                }
            }
            else if (MessageHandler.GetMessageType(data) == MessageType.Position)
            {
                Position message = new NetPosition(data).Deserialized();

                UpdatePosition(message.objId, message.position);
            }
            else if (MessageHandler.GetMessageType(data) == MessageType.Disconnect)
            {
                int disconnectedID = new NetDisconnect(data).Deserialized();
                
                Destroy(spawnedObjects[disconnectedID].gameObject);
                spawnedObjects.Remove(disconnectedID);
            }
        }

        private SpawnableObject Spawn(int objectNumber, int id)
        {
            SpawnableObject instance = !prefabList[objectNumber] ? null : prefabList[objectNumber].Spawn(this, id);

            if (instance)
                instance.transform.parent = transform;
            
            return instance;
        }

        private void UpdatePosition(int id, Vector3 position)
        {
            if (!spawnedObjects.TryGetValue(id, out SpawnableObject spawnedObject)) return;

            if (spawnedObject.transform.position == position) return;
            
            spawnedObject.transform.position = position;
        }

        public void RequestSpawn(int objNumber)
        {
            if (objNumber < 0 || objNumber >= prefabList.Count)
            {
                Debug.LogWarning(objNumber + " is not a valid object number.");
                return;
            }
            
            SpawnRequest spawnRequest = new()
            {
                spawnableNumber = objNumber
            };
            
            List<SpawnRequest> message = new() { spawnRequest };

            NetworkManager.Instance.SendData(new NetSpawnable(message).Serialize());
        }
    }
}