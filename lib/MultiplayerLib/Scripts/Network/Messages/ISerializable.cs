namespace Multiplayer.Network.Messages
{
    public interface ISerializable
    {
        byte[] Serialize();
    }
}