using Game.GameBoard;
using Multiplayer.Network.Objects;
using Multiplayer.Reflection;
using UnityEngine;

namespace Objects
{
    public abstract class BoardPiece : ObjectM
    {
        public abstract bool CanMove { get; protected set; }
        public abstract string Name { get; protected set; }

        [Sync] public int x;
        [Sync] public int y;
        
        public bool MoveTo(Tile tile)
        {
            if (tile == null) return false;
            
            if (Mathf.Abs(x - tile.X) + Mathf.Abs(y - tile.Y) > 1) return false;

            x = tile.X;
            y = tile.Y;
            
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
    }
}