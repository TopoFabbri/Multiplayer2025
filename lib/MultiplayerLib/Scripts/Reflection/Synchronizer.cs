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

            foreach (FieldInfo fieldInfo in node.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                SyncAttribute syncAttribute = fieldInfo.GetCustomAttribute<SyncAttribute>();
                
                if (syncAttribute == null)
                    continue;
                
                SynchronizeNode(node, iterators, fieldInfo, syncAttribute);

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
        
        private static void SynchronizeNode(object node, List<int> iterators, FieldInfo fieldInfo, SyncAttribute syncAttribute, int subIndex = 0)
        {
            if (fieldInfo.FieldType.IsPrimitive || fieldInfo.FieldType.IsEnum)
                SyncPrimitive(node, fieldInfo, iterators, syncAttribute, subIndex);
            else if (typeof(IDictionary).IsAssignableFrom(fieldInfo.FieldType))
                SyncDictionary(node, fieldInfo, iterators, syncAttribute);
            else if (fieldInfo.FieldType != typeof(string) && (fieldInfo.FieldType.IsArray || typeof(ICollection).IsAssignableFrom(fieldInfo.FieldType)))
                SyncCollection(node, fieldInfo, iterators);
            else
                SyncComplex(node, fieldInfo, iterators);
        }

        #region Sync Handlers

        private static void SyncPrimitive(object node, FieldInfo fieldInfo, List<int> iterators, SyncAttribute syncAttribute, int subIndex)
        {
            if (IncomingData.ContainsKey(iterators))
            {
                fieldInfo.SetValue(node, IncomingData[iterators]);
                IncomingData.Remove(iterators);
            }
            
            if (DirtyRegistry.IsDirty(new Node(iterators, subIndex), fieldInfo.GetHashCode()))
            {
                DirtyQueue.Enqueue(PrimitiveSerializer.Serialize(fieldInfo.GetValue(node), fieldInfo.FieldType, syncAttribute.flags, iterators));
            }
        }
        
        private static void SyncCollection(object node, FieldInfo fieldInfo, List<int> iterators)
        {
            foreach (object item in (ICollection)fieldInfo.GetValue(node))
                Synchronize(item, iterators);
        }
        
        private static void SyncDictionary(object node, FieldInfo fieldInfo, List<int> iterators, SyncAttribute syncAttribute)
        {
            int subIndex = 0;
            
            foreach (DictionaryEntry entry in (IDictionary)fieldInfo.GetValue(node))
            {
                iterators.Add(0);
                SynchronizeNode(entry.Key, iterators, fieldInfo, syncAttribute, subIndex);
                iterators[^1]++;
                SynchronizeNode(entry.Value, iterators, fieldInfo, syncAttribute, subIndex);
                
                subIndex++;
            }
        }
        
        private static void SyncComplex(object node, FieldInfo fieldInfo, List<int> iterators)
        {
            Synchronize(fieldInfo.GetValue(node), iterators);
        }

        #endregion
    }
}