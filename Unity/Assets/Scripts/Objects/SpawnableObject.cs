using Multiplayer.NetworkFactory;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

namespace Objects
{
    public abstract class SpawnableObject : MonoBehaviour, ISpawnable
    {
        public SpawnableObjectData Data { get; private set; }

        public virtual void Spawn(SpawnableObjectData data)
        {
            Data = data;
            
            MoveTo(Data.Pos.x, Data.Pos.y, Data.Pos.z);
            RotateTo(Data.Rot);
        }

        public void MoveTo(float x, float y, float z)
        {
            transform.position = new Vector3(x, y, z);
        }

        public void RotateTo(Quaternion quaternion)
        {
            transform.rotation = new UnityEngine.Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}