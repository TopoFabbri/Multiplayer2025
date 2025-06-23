using Multiplayer.Reflection;

namespace Multiplayer.Network.Objects
{
    public class ObjectM
    {
        [Sync] private float posX = 0f;
        [Sync] private float posY = 0f;
        [Sync] private float posZ = 0f;

        [Sync] private int ownerId;
        [Sync] private int objectId;

        ~ObjectM()
        {
        }
        
        public virtual void Initialize(int ownerId, int objectId)
        {
            this.objectId = objectId;
            this.ownerId = ownerId;
        }
        
        public void SetPosition(float x, float y, float z)
        {
            posX = x;
            posY = y;
            posZ = z;
        }
    }
}