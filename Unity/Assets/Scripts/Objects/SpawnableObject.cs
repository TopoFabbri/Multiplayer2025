using UnityEngine;

namespace Objects
{
    public abstract class SpawnableObject : MonoBehaviour
    {
        public int ID { get; private set; }
        protected ObjectManager objectManager;
        
        public virtual SpawnableObject Spawn(ObjectManager objectManager, int id)
        {
            SpawnableObject instance = Instantiate(this);
            instance.transform.position = Vector3.zero;
            instance.objectManager = objectManager;
            instance.ID = id;
            
            return instance;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}