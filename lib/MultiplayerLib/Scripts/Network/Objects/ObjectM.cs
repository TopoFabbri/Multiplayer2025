using Multiplayer.Reflection;

namespace Multiplayer.Network.Objects
{
    public class ObjectM : INetObject
    {
    [Sync] private float posX = 0f;
    [Sync] private float posY = 0f;
    [Sync] private float posZ = 0f;

    [Sync] private int objectId;

    public float PosX => posX;
    public float PosY => posY;
    public float PosZ => posZ;

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

    public int Owner { get; set; }
    }
}