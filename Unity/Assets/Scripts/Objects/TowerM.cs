namespace Objects
{
    public class TowerM : BoardPiece
    {
        ~TowerM()
        {
        }

        public override bool CanMove { get; protected set; } = false;
        public override string Name { get; protected set; } = "Tower";

        public override void Initialize(int ownerId, int objectId)
        {
            base.Initialize(ownerId, objectId);

            Life = 100;
        }
    }
}