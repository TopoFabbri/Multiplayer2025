using UnityEngine;

namespace Objects
{
    public abstract class SpawnableObject : MonoBehaviour
    {
        public int ID { get; set; }
        protected ObjectManager objectManager;
        
        public virtual SpawnableObject Spawn(ObjectManager objectManager)
        {
            SpawnableObject instance = Instantiate(this);
            instance.transform.position = Vector3.zero;
            instance.objectManager = objectManager;
            
            return instance;
        }
    }
}