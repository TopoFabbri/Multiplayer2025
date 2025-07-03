using Multiplayer.Network.Objects;

namespace Objects
{
    public class PawnM : BoardPiece, IDamageable
    {
        public int Life { get; set; } = 50;

        public override void Initialize(int ownerId, int objectId)
        {
            base.Initialize(ownerId, objectId);

            CanMove = true;
        }
    }
}