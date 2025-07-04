using System;
using System.Collections.Generic;

namespace Multiplayer.Reflection
{
    public class Node: IEquatable<Node>
    {
        public List<int> Path { get; } = new();
        
        public Node(List<int> path)
        {
            foreach (int i in path)
                Path.Add(i);
        }
        
        public bool Equals(Node other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (Path.Count != other.Path.Count)
                return false;

            for (int i = 0; i < Path.Count; i++)
            {
                if (Path[i] != other.Path[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Node node && Equals(node);
        }

        public override int GetHashCode()
        {
            HashCode hash = new();
            
            foreach (int i in Path)
                hash.Add(i);
            
            return hash.ToHashCode();
        }
    }
}