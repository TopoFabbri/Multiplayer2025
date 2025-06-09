using System.Collections.Generic;
using Multiplayer.Reflection;

namespace Multiplayer.Network.Objects
{
    public class Model
    {
        public void Update()
        {
            Synchronizer.Synchronize(this, new List<int>());
        }
    }
}