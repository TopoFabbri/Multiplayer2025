using Multiplayer.NetworkFactory;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;
using Vector3 = Multiplayer.CustomMath.Vector3;

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
            Data.Pos = new Vector3(x, y, z);
        }

        public void RotateTo(Vector2 vector2)
        {
            Data.Rot = vector2;
        }

        protected virtual void Update()
        {
            ApplyPosition();
            ApplyRotation();
        }

        protected abstract void ApplyRotation();
        protected abstract void ApplyPosition();
        
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}