namespace Multiplayer.Reflection
{
    public interface INetObject
    {
        int ObjectId { get; }
        int Owner { get; set; }
    }
}