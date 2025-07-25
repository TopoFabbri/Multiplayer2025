using Multiplayer.Reflection;

namespace Multiplayer.Network.Objects
{
    public class ObjectM : INetObject
    {
        [Sync] protected float posX = 0f;
        [Sync] protected float posY = 0f;
        [Sync] protected float posZ = 0f;

        [Sync] protected int objectId;
        [Sync] protected bool isActive = true;

        public float PosX => posX;
        public float PosY => posY;
        public float PosZ => posZ;
        public bool IsActive => isActive;

        public int ObjectId => objectId;

        ~ObjectM()
        {
        }

        public virtual void Initialize(int ownerId, int objectId)
        {
            this.objectId = objectId;
            Owner = ownerId;
        }

        public void SetPosition(float x, float y, float z)
        {
            posX = x;
            posY = y;
            posZ = z;
        }

        public virtual void Update()
        {
        }

        public int Owner { get; set; }
    }
}