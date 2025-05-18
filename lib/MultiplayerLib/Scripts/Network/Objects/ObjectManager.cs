using System.Collections.Generic;
using System.Numerics;
using Multiplayer.NetworkFactory;

namespace Multiplayer.Network.Objects
{
    public class ObjectManager : INetworkFactory
    {
        private readonly Dictionary<int, Spawnable> spawnablesById = new();
        
        public void SpawnObject(SpawnableObjectData data)
        {
            Spawnable spawnable = new();
            spawnable.Spawn(data);
            
            spawnablesById.Add(data.Id, spawnable);
        }

        public void MoveObjectTo(int id, float x, float y, float z)
        {
            spawnablesById[id].MoveTo(x, y, z);
        }

        public void RotateObjectTo(int id, Quaternion rot)
        {
            spawnablesById[id].RotateTo(rot);
        }
        
        public void DestroyObject(int id)
        {
            spawnablesById[id].Destroy();
        }
    }
}