using Game;
using Multiplayer.Reflection;
using UnityEngine;

namespace Objects
{
    public class TowerM : BoardPiece, IDamageable
    {
        public int Life { get; set; }

        ~TowerM()
        {
            InputListener.Space -= ShoutId;
        }
        
        public override void Initialize(int ownerId, int objectId)
        {
            base.Initialize(ownerId, objectId);
            
            InputListener.Space += ShoutId;
        }

        [Rpc] private void ShoutId()
        {
            Debug.Log("ID: " + ObjectId + ", Owner: " + Owner);
        }

        public override bool CanMove { get; protected set; } = false;
        public override string Name { get; protected set; } = "Tower";
    }
}