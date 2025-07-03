using Multiplayer.Network.Objects;
using Multiplayer.NetworkFactory;

namespace Objects
{
    public class PawnV : ObjectV
    {
        public override ObjectM Initialize(SpawnableObjectData data)
        {
            Model = new PawnM();
            
            Model.Initialize(data.OwnerId, data.Id);

            return Model;
        }
    }
}