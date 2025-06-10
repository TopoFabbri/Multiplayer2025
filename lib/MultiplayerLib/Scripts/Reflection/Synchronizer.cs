using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Multiplayer.Network;
using Multiplayer.Network.Messages.Primitives;

namespace Multiplayer.Reflection
{
    public static class Synchronizer
    {
        private static readonly Queue<byte[]> DirtyQueue = new();

        public static void Synchronize(object baseNode, List<int> iterators)
        {
            iterators.Add(0);

            foreach (FieldInfo field in baseNode.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                SyncAttribute syncAttribute = field.GetCustomAttribute<SyncAttribute>();
                
                if (syncAttribute == null)
                    continue;
                
                if (field.FieldType.IsPrimitive || field.FieldType.IsEnum)
                {
                    if (DirtyRegistry.IsDirty(new Node(iterators), field.GetHashCode()))
                        DirtyQueue.Enqueue(PrimitiveSerializer.Serialize(field.GetValue(baseNode), syncAttribute.flags, iterators));
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
        
        public static byte[] DequeueDirty()
        {
            return DirtyQueue.Count == 0 ? null : DirtyQueue.Dequeue();
        }
    }
}