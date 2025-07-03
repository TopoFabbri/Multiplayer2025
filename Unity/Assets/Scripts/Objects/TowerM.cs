namespace Objects
{
    public class TowerM : BoardPiece, IDamageable
    {
        public int Life { get; set; }

        public override void Initialize(int ownerId, int objectId)
        {
            base.Initialize(ownerId, objectId);

            CanMove = false;
        }
    }
}