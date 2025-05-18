using System.Numerics;
using Multiplayer.NetworkFactory;
using Vector3 = Multiplayer.CustomMath.Vector3;

namespace Multiplayer.Network.Objects
{
    public class Spawnable : ISpawnable
    {
        public SpawnableObjectData Data { get; private set; }

        public void Spawn(SpawnableObjectData data)
        {
            Data = data;
        }

        public void MoveTo(float x, float y, float z)
        {
            Data.Pos = new Vector3(x, y, z);
        }

        public void RotateTo(Quaternion quaternion)
        {
            Data.Rot = quaternion;
        }

        public void Destroy()
        {
        }
    }
}