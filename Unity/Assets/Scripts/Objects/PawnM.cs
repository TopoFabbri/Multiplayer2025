using Multiplayer.Reflection;

namespace Objects
{
    public class PawnM : BoardPiece, IDamageable
    {
        [field: Sync] public int Life { get; set; } = 50;

        public override bool CanMove { get; protected set; } = true;
        public override string Name { get; protected set; } = "Infantry";
    }
}