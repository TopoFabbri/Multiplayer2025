namespace Multiplayer.NetworkFactory
{
    public interface INetworkFactory
    {
        public void SpawnObject(SpawnableObjectData data);
        public void DestroyObject(int id);
    }
}