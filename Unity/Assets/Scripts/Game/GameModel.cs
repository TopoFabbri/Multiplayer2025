using Multiplayer.Network.Objects;
using Multiplayer.NetworkFactory;
using Multiplayer.Reflection;
using Objects;

namespace Game
{
    public class GameModel : Model
    {
        [Sync] private ModelObjectManager objectManager;

        public GameModel(ModelObjectManager objectManager)
        {
            this.objectManager = objectManager;
        }
        
        public void SpawnObject(SpawnableObjectData data)
        {
            objectManager.SpawnObject(data);
        }

        public void RequestSpawn(SpawnableObjectData spawnableData)
        {
            objectManager.RequestSpawn(spawnableData);
        }
    }
}