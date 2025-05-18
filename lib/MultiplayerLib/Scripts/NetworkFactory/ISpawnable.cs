using System.Numerics;

namespace Multiplayer.NetworkFactory
{
    public interface ISpawnable
    {
        public void Spawn(SpawnableObjectData data);
        public void MoveTo(float x, float y, float z);
        public void RotateTo(Vector2 vector2);
        public void Destroy();
    }
}