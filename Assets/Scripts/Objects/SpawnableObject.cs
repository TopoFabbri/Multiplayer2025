using UnityEngine;

namespace Objects
{
    public abstract class SpawnableObject : MonoBehaviour
    {
        public int ID { get; set; }
        
        public virtual SpawnableObject Spawn()
        {
            SpawnableObject instance = Instantiate(this);
            instance.transform.position = Vector3.zero;
            
            return instance;
        }
    }
}