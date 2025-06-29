using System.Collections.Generic;

namespace Multiplayer.Reflection
{
    public static class DirtyRegistry
    {
        private static readonly Dictionary<Node, int> HashValues = new();

        public static bool IsDirty(Node node, int hash)
        {
            if (HashValues.TryAdd(node, hash)) return true;

            if (HashValues[node] == hash) return false;
        
            HashValues[node] = hash;
            return true;
        }
        
        public static void SetHash(Node node, int hash)
        {
            HashValues[node] = hash;
        }
    }
}