using System.Collections.Generic;

namespace Multiplayer.Reflection
{
    public static class DirtyRegistry
    {
        public static Dictionary<Node, object> PrevValues { get; } = new(); 

        public static bool IsDirty(Node node, object obj)
        {
            if (PrevValues.TryAdd(node, obj)) return true;

            if (PrevValues[node].GetHashCode() == obj.GetHashCode()) return false;
        
            return true;
        }
        
        public static void UpdateNode(Node node, object obj)
        {
            PrevValues[node] = obj;
        }
    }
}