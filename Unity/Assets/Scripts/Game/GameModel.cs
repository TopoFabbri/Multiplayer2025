using System.Collections.Generic;
using Multiplayer.NetworkFactory;
using Multiplayer.Reflection;
using Objects;

namespace Game
{
    public class GameModel
    {
        [Sync] private ModelObjectManager objectManager;

        public void Update()
        {
            Synchronizer.Synchronize(this, new List<int>());
        }

        public void SpawnObject(SpawnableObjectData data)
        {
            objectManager.SpawnObject(data);
        }
    }
}