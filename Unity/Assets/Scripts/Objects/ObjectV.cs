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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
                model.SetPosition(model.PosX - .5f, model.PosY, model.PosZ);
            if (Input.GetKeyDown(KeyCode.D))
                model.SetPosition(model.PosX + .5f, model.PosY, model.PosZ);
            
            if (Input.GetKeyDown(KeyCode.S))
                model.SetPosition(model.PosX, model.PosY, model.PosZ - .5f);
            if (Input.GetKeyDown(KeyCode.W))
                model.SetPosition(model.PosX, model.PosY, model.PosZ + .5f);
        }

        private void LateUpdate()
        {
            transform.position = new Vector3(model.PosX, model.PosY, model.PosZ);
        }
    }
}