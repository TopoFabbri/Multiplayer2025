using System.Collections.Generic;
using System.Linq;
using System.Net;
using Network;
using Network.Messages;
using UnityEngine;

namespace Objects
{
    public class ObjectManager : MonoBehaviour
    {
        [SerializeField] private List<SpawnableObject> prefabList = new();

        private readonly Dictionary<int, SpawnableObject> spawnedObjects = new();
        
        private readonly Dictionary<int, int> lastPosMessageByObjId = new();
        
        private void OnEnable()
        {
            MessageHandler.TryAddHandler(MessageType.SpawnRequest, HandleSpawnRequest);
            MessageHandler.TryAddHandler(MessageType.Position, HandlePosition);
            MessageHandler.TryAddHandler(MessageType.Disconnect, HandleDisconnect);
        }
        
        private void OnDisable()
        {
            MessageHandler.TryRemoveHandler(MessageType.SpawnRequest, HandleSpawnRequest);
            MessageHandler.TryRemoveHandler(MessageType.Position, HandlePosition);
            MessageHandler.TryRemoveHandler(MessageType.Disconnect, HandleDisconnect);
        }

        private void HandleDisconnect(byte[] data, IPEndPoint ip)
        {
                int disconnectedID = new NetDisconnect(data).Deserialized();
                
                Destroy(spawnedObjects[disconnectedID].gameObject);
                spawnedObjects.Remove(disconnectedID);
        }

        private void HandleSpawnRequest(byte[] data, IPEndPoint ip)
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

        private void HandlePosition(byte[] data, IPEndPoint ip)
        {
            NetPosition message = new(data);
                
            lastPosMessageByObjId.TryAdd(message.Data.objId, 0);
                
            if (message.Metadata.Id < lastPosMessageByObjId[message.Data.objId])
                return;

            UpdatePosition(message.Data.objId, message.Data.position);
                
            lastPosMessageByObjId[message.Data.objId] = message.Metadata.Id;
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