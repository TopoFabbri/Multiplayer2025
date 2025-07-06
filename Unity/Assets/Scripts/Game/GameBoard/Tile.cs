using System;

namespace Game.GameBoard
{
    public class Tile : IEquatable<Tile>
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Tile other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Tile)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public void Set(int tileX, int tileY)
        {
            X = tileX;
            Y = tileY;
        }
    }
}