using System;
using System.Collections.Generic;

namespace Multiplayer.Reflection
{
    public class Node: IEquatable<Node>
    {
        public List<int> Path { get; }
        public int SubIndex { get; }
        
        public Node(List<int> path)
        {
            Path = path;
            SubIndex = 0;
        }
        
        public Node(List<int> path, int subIndex)
        {
            Path = path;
            SubIndex = subIndex;
        }

        public bool Equals(Node other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (Path.Count != other.Path.Count || SubIndex != other.SubIndex)
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
            
            hash.Add(SubIndex);
            
            foreach (int i in Path)
                hash.Add(i);
            
            return hash.ToHashCode();
        }
    }
}