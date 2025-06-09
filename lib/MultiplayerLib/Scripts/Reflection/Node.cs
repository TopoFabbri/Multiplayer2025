using System.Collections.Generic;

namespace Multiplayer.Reflection
{
    public class Node
    {
        public List<int> Path { get; }
        public List<int> Indexes { get; }
        
        public Node(List<int> path)
        {
            Path = path;
            Indexes = new List<int>();
        }
        
        public Node(List<int> path, List<int> indexes)
        {
            Path = path;
            Indexes = indexes;
        }
    }
}