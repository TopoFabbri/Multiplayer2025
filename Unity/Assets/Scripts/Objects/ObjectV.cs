using Multiplayer.Network.Objects;
using UnityEngine;

namespace Objects
{
    public class ObjectV : MonoBehaviour
    {
        private ObjectM model;
        
        public void Initialize(ObjectM model)
        {
            this.model = model;
        }
    }
}