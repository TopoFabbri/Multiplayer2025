using System;
using Interfaces;
using Multiplayer.Network.Objects;
using UnityEngine;

namespace Objects
{
    public class ObjectV : MonoBehaviour, IObjectView
    {
        public ObjectM Model { get; set; }

        public virtual void Initialize(ObjectM model)
        {
            Model = model;
        }

        private void LateUpdate()
        {
            transform.position = new Vector3(Model.PosX, Model.PosY, Model.PosZ) * 2f;
        }
        
        public virtual Type ModelType => typeof(ObjectM);
    }
}