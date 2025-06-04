using System.Collections.Generic;
using System.Net;
using Game;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.NetworkFactory;
using UnityEngine;
using Utils;

namespace Objects
{
    public class ObjectManager : MonoBehaviourSingleton<ObjectManager>, INetworkFactory
    {
        [SerializeField] private List<SpawnableObject> prefabList = new();

        private readonly Dictionary<int, SpawnableObject> spawnedObjects = new();

        private readonly Dictionary<string, GameObject> parentsByPrefabName = new();

        private void OnEnable()
        {
            MessageHandler.TryAddHandler(MessageType.SpawnRequest, HandleSpawnRequest);
            MessageHandler.TryAddHandler(MessageType.Despawn, HandleDespawn);

            GameStateController.StateChanged += OnStateChanged;
        }

        private void OnStateChanged(GameState newState)
        {
            if (newState != GameState.InGame)
                Disconnect();
        }

        private void OnDisable()
        {
            MessageHandler.TryRemoveHandler(MessageType.SpawnRequest, HandleSpawnRequest);
            MessageHandler.TryRemoveHandler(MessageType.Despawn, HandleDespawn);
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

            GameObject newParent = new(prefabName + "s") { transform = { rotation = Quaternion.identity, position = Vector3.zero } };

            parentsByPrefabName.Add(prefabName, newParent);
            return newParent;
        }

        private void HandleDespawn(byte[] data, IPEndPoint ip)
        {
            int objId = new NetDespawn(data).Deserialized();

            if (spawnedObjects.ContainsKey(objId))
            {
                spawnedObjects[objId].Destroy();
                spawnedObjects.Remove(objId);
            }
        }

        public void SpawnObject(SpawnableObjectData data)
        {
            if (spawnedObjects.ContainsKey(data.Id))
                return;

            SpawnableObject spawnedObject = Instantiate(prefabList[data.PrefabId], GetParent(prefabList[data.PrefabId].name).transform);

            spawnedObject.Spawn(data);
            spawnedObjects.Add(data.Id, spawnedObject);
        }

        public void RequestSpawn(SpawnableObjectData spawnableData)
        {
            if (spawnableData.PrefabId < 0 || spawnableData.PrefabId >= prefabList.Count)
            {
                Debug.LogWarning(spawnableData.PrefabId + " is not a valid object number.");
                return;
            }

            SpawnRequest spawnRequest = new(new List<SpawnableObjectData> { spawnableData });

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
            if (spawnedObjects.TryGetValue(id, out SpawnableObject obj))
                obj.Destroy();
        }
    }
}