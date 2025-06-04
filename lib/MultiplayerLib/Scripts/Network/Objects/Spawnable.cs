using Multiplayer.NetworkFactory;

namespace Multiplayer.Network.Objects
{
    public class Spawnable : ISpawnable
    {
        public SpawnableObjectData Data { get; private set; }

        public void Spawn(SpawnableObjectData data)
        {
            Data = data;
        }

        public void Destroy()
        {
        }
    }
}