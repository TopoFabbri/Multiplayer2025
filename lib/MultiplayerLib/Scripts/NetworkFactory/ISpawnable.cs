using System.Numerics;

namespace Multiplayer.NetworkFactory
{
    public interface ISpawnable
    {
        public void Spawn(SpawnableObjectData data);
        public void MoveTo(float x, float y, float z);
        public void RotateTo(Quaternion quaternion);
        public void Destroy();
    }
}