using System.Numerics;

namespace Multiplayer.NetworkFactory
{
    public interface INetworkFactory
    {
        public void SpawnObject(SpawnableObjectData data);
        public void MoveObjectTo(int id, float x, float y, float z);
        public void RotateObjectTo(int id, Quaternion rot);
        public void DestroyObject(int id);
    }
}