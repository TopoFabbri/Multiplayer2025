using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Multiplayer.Network.Messages.Primitives;

namespace Multiplayer.Reflection
{
    public static class Synchronizer
    {
        private static readonly Queue<byte[]> DirtyQueue = new();
        private static readonly Dictionary<List<int>, object> IncomingData = new();

        public static void Synchronize(object node, List<int> iterators)
        {
            iterators.Add(0);

            foreach (FieldInfo field in node.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                SyncAttribute syncAttribute = field.GetCustomAttribute<SyncAttribute>();
                
                if (syncAttribute == null)
                    continue;
                
                if (field.FieldType.IsPrimitive || field.FieldType.IsEnum)
                {
                    if (IncomingData.ContainsKey(iterators))
                    {
                        field.SetValue(node, IncomingData[iterators]);
                        IncomingData.Remove(iterators);
                    }
                    
                    if (DirtyRegistry.IsDirty(new Node(iterators), field.GetHashCode()))
                        DirtyQueue.Enqueue(PrimitiveSerializer.Serialize(field.GetValue(node), field.FieldType, syncAttribute.flags, iterators));
                }
                else if (field.FieldType != typeof(string) && (field.FieldType.IsArray || typeof(ICollection).IsAssignableFrom(field.FieldType)))
                {
                    foreach (object item in (ICollection)field.GetValue(node))
                        Synchronize(item, iterators);
                }
                else
                {
                    Synchronize(field.GetValue(node), iterators);
                }

                iterators[^1]++;
            }

            iterators.RemoveAt(iterators.Count - 1);
        }
        
        public static byte[] DequeueDirty()
        {
            return DirtyQueue.Count == 0 ? null : DirtyQueue.Dequeue();
        }

        public static void AddIncomingData(List<int> key, object data)
        {
            IncomingData.Add(key, data);
        }
        
        public static bool HasDirty()
        {
            return DirtyQueue.Count > 0;
        }
    }
}