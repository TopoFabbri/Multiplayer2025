using Multiplayer.Network.Objects;

namespace Interfaces
{
    public interface IObjectView
    {
        ObjectM Model { get; set; }

        public void Initialize(ObjectM model);
    }
}