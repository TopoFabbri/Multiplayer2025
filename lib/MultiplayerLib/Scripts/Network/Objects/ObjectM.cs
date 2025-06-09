using Multiplayer.Reflection;

namespace Multiplayer.Network.Objects
{
    public class ObjectM
    {
        [Sync] private float posX;
        [Sync] private float posY;
        [Sync] private float posZ;

        [Sync] private int ownerId;
        
        public ObjectM()
        {
            posX = 0f;
            posY = 0f;
            posZ = 0f;
        }

        ~ObjectM()
        {
        }
        
        public virtual void Initialize(int ownerId)
        {
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