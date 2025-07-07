using Interfaces;

namespace Objects
{
    public class PawnM : BoardPiece, IDamageable
    {
        public override bool CanMove { get; protected set; } = true;
        public override string Name { get; protected set; } = "Infantry";

        public override void Initialize(int ownerId, int objectId)
        {
            base.Initialize(ownerId, objectId);
            
            Life = 50;
        }
    }
}