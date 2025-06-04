using Multiplayer.NetworkFactory;
using UnityEngine;

namespace Objects
{
    public abstract class SpawnableObject : MonoBehaviour, ISpawnable
    {
        public SpawnableObjectData Data { get; private set; }

        public virtual void Spawn(SpawnableObjectData data)
        {
            Data = data;
        }
        
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}