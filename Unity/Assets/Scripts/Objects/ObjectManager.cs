using System.Collections.Generic;
using System.Net;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.MessageInfo;
using Multiplayer.NetworkFactory;
using UnityEngine;
using Utils;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = Multiplayer.CustomMath.Vector3;

namespace Objects
{
    public class ObjectManager : MonoBehaviourSingleton<ObjectManager>, INetworkFactory
    {
        [SerializeField] private List<SpawnableObject> prefabList = new();

        [SerializeField] private List<Transform> spawns;
        
        private readonly Dictionary<int, SpawnableObject> spawnedObjects = new();
        private readonly Dictionary<int, int> lastPosMessageByObjId = new();

        private readonly Dictionary<string, GameObject> parentsByPrefabName = new();

        private void OnEnable()
        {
            MessageHandler.TryAddHandler(MessageType.SpawnRequest, HandleSpawnRequest);
            MessageHandler.TryAddHandler(MessageType.Position, HandlePosition);
        }

        private void OnDisable()
        {
            MessageHandler.TryRemoveHandler(MessageType.SpawnRequest, HandleSpawnRequest);
            MessageHandler.TryRemoveHandler(MessageType.Position, HandlePosition);
        }

        private void HandleSpawnRequest(byte[] data, IPEndPoint ip)
        {
            SpawnRequest message = new NetSpawnable(data).Deserialized();

            foreach (SpawnableObjectData spawnableObject in message.spawnableObjects)
                SpawnObject(spawnableObject);
        }

        private GameObject GetParent(string prefabName)
        {
            if (parentsByPrefabName.TryGetValue(prefabName, out GameObject parent))
                return parent;

            GameObject newParent = Instantiate(new GameObject(prefabName + "s"));

            newParent.transform.rotation = UnityEngine.Quaternion.identity;
            newParent.transform.position = UnityEngine.Vector3.zero;

            parentsByPrefabName.Add(prefabName, newParent);
            return newParent;
        }
        
        private void HandlePosition(byte[] data, IPEndPoint ip)
        {
            MessageMetadata metadata = MessageMetadata.Deserialize(data);
            Position position = new NetPosition(data).Deserialized();

            lastPosMessageByObjId.TryAdd(position.objId, 0);

            if (metadata.MsgId < lastPosMessageByObjId[position.objId])
                return;

            spawnedObjects[position.objId].MoveTo(position.position.x, position.position.y, position.position.z);

            lastPosMessageByObjId[position.objId] = metadata.MsgId;
        }

        private void HandleRotation(byte[] data, IPEndPoint ip)
        {
            // MessageMetadata metadata = MessageMetadata.Deserialize(data);
            // Rotation rotation = new NetRotation(data).Deserialized();
        }
        
        public void SpawnObject(SpawnableObjectData data)
        {
            if (spawnedObjects.ContainsKey(data.Id))
                return;

            SpawnableObject spawnedObject = Instantiate(prefabList[data.PrefabId], GetParent(prefabList[data.PrefabId].name).transform);

            spawnedObject.Spawn(data);
            spawnedObjects.Add(data.Id, spawnedObject);
            
            spawnedObject.transform.position = spawns[spawnedObject.Data.Id % spawns.Count].position;
        }

        public void RequestSpawn(int objNumber)
        {
            if (objNumber < 0 || objNumber >= prefabList.Count)
            {
                Debug.LogWarning(objNumber + " is not a valid object number.");
                return;
            }

            SpawnableObjectData data = new() { OwnerId = NetworkManager.Instance.Id, PrefabId = objNumber, Pos = Vector3.Zero, Rot = Quaternion.Identity };

            SpawnRequest spawnRequest = new(new List<SpawnableObjectData> { data });

            NetworkManager.Instance.SendData(new NetSpawnable(spawnRequest).Serialize());
        }

        public void Disconnect()
        {
            foreach (KeyValuePair<int, SpawnableObject> spawnedObj in spawnedObjects)
                spawnedObj.Value.Destroy();

            spawnedObjects.Clear();
        }

        public void DestroyObject(int id)
        {
            spawnedObjects[id].Destroy();
        }
    }
}