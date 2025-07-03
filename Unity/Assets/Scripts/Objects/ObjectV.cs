using Multiplayer.Network.Objects;
using Multiplayer.NetworkFactory;
using UnityEngine;

namespace Objects
{
    public class ObjectV : MonoBehaviour, IObjectView
    {
        public ObjectM Model { get; set; }

        public virtual ObjectM Initialize(SpawnableObjectData data)
        {
            Model = new ObjectM();
            
            Model.Initialize(data.OwnerId, data.Id);

            return Model;
        }

        private void LateUpdate()
        {
            transform.position = new Vector3(Model.PosX, Model.PosY, Model.PosZ) * 2f;
        }
    }
}