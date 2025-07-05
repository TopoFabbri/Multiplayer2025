using Multiplayer.Network.Objects;

namespace Multiplayer.NetworkFactory
{
    public interface INetworkFactory
    {
        public ObjectM SpawnObject(SpawnableObjectData data);
        public void DestroyObject(int id);
    }
}