using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Multiplayer.Reflection
{
    public static class Synchronizer
    {
        public static void Synchronize(object baseNode, List<int> iterators)
        {
            iterators.Add(0);
            
            foreach (FieldInfo field in baseNode.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.FieldType.IsPrimitive || field.FieldType.IsEnum)
                {
                    if (DirtyRegistry.IsDirty(new Node(iterators), field.GetHashCode()))
                    {
                        // TODO: Check if owner end field value to clients
                    }
                }
                else if (field.FieldType != typeof(string) && (field.FieldType.IsArray || typeof(ICollection).IsAssignableFrom(field.FieldType)))
                {
                    foreach (object item in field.GetValue(baseNode) as ICollection)
                        Synchronize(item, iterators);
                }
                else
                {
                    Synchronize(field.GetValue(baseNode), iterators);
                }

                iterators[^1]++;
            }
            
            iterators.RemoveAt(iterators.Count - 1);
        }
    }
}