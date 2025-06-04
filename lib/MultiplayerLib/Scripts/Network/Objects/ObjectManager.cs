using System.Collections.Generic;
using System.Numerics;
using Multiplayer.NetworkFactory;
using Vector3 = Multiplayer.CustomMath.Vector3;

namespace Multiplayer.Network.Objects
{
    public class ObjectManager : INetworkFactory
    {
        private readonly Dictionary<int, Spawnable> spawnedById = new();
        
        public int FreeId
        {
            get
            {
                int id = 0;
                
                while (spawnedById.ContainsKey(id)) id++;
                
                return id;
            }
        }
        
        public SpawnableObjectData GetSpawnableData(int id)
        {
            return !spawnedById.TryGetValue(id, out Spawnable obj) ? null : obj.Data;
        }

        public List<SpawnableObjectData> SpawnablesData
        {
            get
            {
                List<SpawnableObjectData> data = new();
                
                foreach (Spawnable spawnable in spawnedById.Values)
                    data.Add(spawnable.Data);
                
                return data;
            }
        }

        public void SpawnObject(SpawnableObjectData data)
        {
            if (spawnedById.ContainsKey(data.Id)) return;
            
            Spawnable spawnable = new();
            spawnable.Spawn(data);
            
            spawnedById.Add(data.Id, spawnable);
        }
        
        public void DestroyObject(int id)
        {
            if (!spawnedById.TryGetValue(id, out Spawnable obj)) return;
            
            obj.Destroy();
            
            spawnedById.Remove(id);
        }
    }
}