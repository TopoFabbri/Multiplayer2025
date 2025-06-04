namespace Multiplayer.NetworkFactory
{
    public interface ISpawnable
    {
        public void Spawn(SpawnableObjectData data);
        public void Destroy();
    }
}