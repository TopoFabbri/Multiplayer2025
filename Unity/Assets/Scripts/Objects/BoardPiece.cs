using Multiplayer.Network.Objects;

namespace Objects
{
    public abstract class BoardPiece : ObjectM
    {
        public abstract bool CanMove { get; protected set; }
        public abstract string Name { get; protected set; }
    }
}