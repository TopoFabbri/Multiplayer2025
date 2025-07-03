using Multiplayer.Network.Objects;
using Multiplayer.NetworkFactory;

namespace Objects
{
    public interface IObjectView
    {
        ObjectM Model { get; set; }

        public ObjectM Initialize(SpawnableObjectData data);
    }
}