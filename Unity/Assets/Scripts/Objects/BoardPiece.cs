using System;
using Game.GameBoard;
using Interfaces;
using Multiplayer.Network.Objects;
using Multiplayer.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Objects
{
    public abstract class BoardPiece : ObjectM, IDamageable
    {
        public abstract bool CanMove { get; protected set; }
        public abstract string Name { get; protected set; }
        [field: Sync] public int Life { get; set; }

        [Sync] public int x;
        [Sync] public int y;

        public static event Action<int> Moved;
        
        public void ReceiveDamage()
        {
            int damage = (int)(Life * 0.2 + 5 + Random.Range(2, 8));
            
            Life -= damage;
        }
        
        public bool MoveTo(Tile tile)
        {
            if (tile == null) return false;
            
            if (Mathf.Abs(x - tile.X) + Mathf.Abs(y - tile.Y) > 1) return false;

            x = tile.X;
            y = tile.Y;
            
            OnMoved();
            return true;
        }
        
        public void PlaceAt(int x, int y)
        {
            this.x = x;
            this.y = y;

            SetPosition(x, 0, y);
        }

        public override void Update()
        {
            base.Update();

            SetPosition(x, 0, y);
        }

        [Rpc] private void OnMoved()
        {
            Moved?.Invoke(objectId);
        }
    }
}