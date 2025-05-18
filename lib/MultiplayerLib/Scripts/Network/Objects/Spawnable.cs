using System.Numerics;
using Multiplayer.NetworkFactory;
using Vector3 = Multiplayer.CustomMath.Vector3;

namespace Multiplayer.Network.Objects
{
    public class Spawnable : ISpawnable
    {
        private SpawnableObjectData data;

        public void Spawn(SpawnableObjectData data)
        {
            this.data = data;
        }

        public void MoveTo(float x, float y, float z)
        {
            data.Pos = new Vector3(x, y, z);
        }

        public void RotateTo(Quaternion quaternion)
        {
            data.Rot = quaternion;
        }

        public void Destroy()
        {
        }
    }
}