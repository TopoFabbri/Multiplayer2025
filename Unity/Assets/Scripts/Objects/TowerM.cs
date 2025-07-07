namespace Objects
{
    public class TowerM : BoardPiece, IDamageable
    {
        public int Life { get; set; }

        ~TowerM()
        {
        }

        public override bool CanMove { get; protected set; } = false;
        public override string Name { get; protected set; } = "Tower";
    }
}