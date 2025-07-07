using Multiplayer.Reflection;

namespace Objects
{
    public class TowerM : BoardPiece, IDamageable
    {
        [field: Sync] public int Life { get; set; } = 100;

        ~TowerM()
        {
        }

        public override bool CanMove { get; protected set; } = false;
        public override string Name { get; protected set; } = "Tower";
    }
}