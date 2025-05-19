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

            if (data.PrefabId == 0)
            {
                Vector3 pos = data.Pos;
            
                pos.x += data.Id * 2;
                data.Pos = pos;
            }
            
            Spawnable spawnable = new();
            spawnable.Spawn(data);
            
            spawnedById.Add(data.Id, spawnable);
        }

        public void MoveObjectTo(int id, float x, float y, float z)
        {
            if (!spawnedById.TryGetValue(id, out Spawnable obj)) return;
            
            obj.MoveTo(x, y, z);
        }

        public void RotateObjectTo(int id, Vector2 vector2)
        {
            if (!spawnedById.TryGetValue(id, out Spawnable obj)) return;
                obj.RotateTo(vector2);
        }
        
        public void DestroyObject(int id)
        {
            if (!spawnedById.TryGetValue(id, out Spawnable obj)) return;
            
            obj.Destroy();
            
            spawnedById.Remove(id);
        }
    }
}