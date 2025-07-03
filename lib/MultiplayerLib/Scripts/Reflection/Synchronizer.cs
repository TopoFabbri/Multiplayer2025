using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Multiplayer.Network;
using Multiplayer.Network.Messages.Primitives;

namespace Multiplayer.Reflection
{
    /* * * * * * * * * * * * * * * * * * * * * * Graph * * * * * * * * * * * * * * * * * * * * *
     * ┌───┐                                                                                   *
     * │ 0 │                            Model                        0              │   0      *
     * └╥──┘┌─────┐                                                  ├── 0          │   00     *
     *  ╠═══╡ 0-0 │                     Primitive                    ├── 1          │   01     *
     *  ║   ├─────┤                                                  │   ├── 0      │   010    *
     *  ╠═══╡ 0-1 │                     Collection                   │   └── 1      │   011    *
     *  ║   └╥────┘┌───────┐                                         │              │          *
     *  ║    ╠═════╡ 0-1-0 │            First collection item        ├── 2          │   02     *
     *  ║    ║     ├───────┤                                         │   ├── 0      │   020    *
     *  ║    ╚═════╡ 0-1-1 │            Second collection item       │   │   ├── 0  │   0200   *
     *  ║   ┌─────┐└───────┘                                         │   │   └── 1  │   0201   *
     *  ╠═══╡ 0-2 │                     Dictionary                   │   │          │          *
     *  ║   └╥────┘┌───────┐                                         │   └── 1      │   021    *
     *  ║    ╠═════╡ 0-2-0 │            First dictionary item        │       ├── 0  │   0210   *
     *  ║    ║     └╥──────┘┌─────────┐                              │       └── 1  │   0211   *
     *  ║    ║      ╠═══════╡ 0-2-0-0 │ First dictionary key         │              │          *
     *  ║    ║      ║       ├─────────┤                              └─── 3         │   03     *
     *  ║    ║      ╚═══════╡ 0-2-0-1 │ First dictionary value            └── 0     │   030    *
     *  ║    ║     ┌───────┐└─────────┘                                                        *
     *  ║    ╚═════╡ 0-2-1 │            Second dictionary item                                 *
     *  ║          └╥──────┘┌─────────┐                                                        *
     *  ║           ╠═══════╡ 0-2-1-0 │ Second dictionary key                                  *
     *  ║           ║       ├─────────┤                                                        *
     *  ║           ╚═══════╡ 0-2-1-1 │ Second dictionary value                                *
     *  ║    ┌─────┐        └─────────┘                                                        *
     *  ╠════╡ 0-3 │                    Complex object                                         *
     *  ║    └╥────┘┌───────┐                                                                  *
     *  ║     ╚═════╡ 0-3-0 │           First complex primitive                                *
     *  ║    ┌─────┐└───────┘                                                                  *
     *  ╚════╡ 0-4 │                    Complex object                                         *
     *       └╥────┘┌───────┐                                                                  *
     *        ╚═════╡ 0-4-0 │           First complex primitive                                *
     *              └───────┘                                                                  *
     * * * * * * * * * * * * * * * * * * * * * Reference * * * * * * * * * * * * * * * * * * * */

    public static class Synchronizer
    {
        private static readonly Queue<byte[]> DirtyQueue = new();
        private static readonly Dictionary<Node, object> IncomingData = new();

        public static void Synchronize(object node, List<int> iterators)
        {
            iterators.Add(0);

            int owner = typeof(INetObject).IsAssignableFrom(node.GetType()) ? ((INetObject)node).Owner : 0;

            Type nodeType = node.GetType();

            while (nodeType != null)
            {
                FieldInfo[] fields = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (FieldInfo fieldInfo in fields)
                {
                    SyncAttribute syncAttribute = fieldInfo.GetCustomAttribute<SyncAttribute>();

                    if (syncAttribute == null)
                        continue;

                    SynchronizeNode(node, iterators, fieldInfo, syncAttribute, owner);

                    iterators[^1]++;
                }

                nodeType = nodeType.BaseType;
            }

            iterators.RemoveAt(iterators.Count - 1);
        }

        public static byte[] DequeueDirty()
        {
            return DirtyQueue.Count == 0 ? null : DirtyQueue.Dequeue();
        }

        public static void AddIncomingData(List<int> key, object data)
        {
            Node keyNode = new(key);

            IncomingData[keyNode] = data;
        }

        public static bool HasDirty()
        {
            return DirtyQueue.Count > 0;
        }

        private static void SynchronizeNode(object node, List<int> iterators, FieldInfo fieldInfo, SyncAttribute syncAttribute, int owner)
        {
            if (fieldInfo.FieldType.IsPrimitive || fieldInfo.FieldType.IsEnum)
                SyncPrimitiveField(node, fieldInfo, iterators, syncAttribute, owner);
            else if (typeof(IDictionary).IsAssignableFrom(fieldInfo.FieldType))
                SyncDictionaryField(node, fieldInfo, iterators, syncAttribute, owner);
            else if (fieldInfo.FieldType != typeof(string) && (fieldInfo.FieldType.IsArray || typeof(ICollection).IsAssignableFrom(fieldInfo.FieldType)))
                SyncCollectionField(node, fieldInfo, iterators, syncAttribute, owner);
            else
                SyncComplexField(node, fieldInfo, iterators);
        }

        #region Sync Handlers

        private static void SyncPrimitiveField(object node, FieldInfo fieldInfo, List<int> iterators, SyncAttribute syncAttribute, int owner)
        {
            Node curNode = new(iterators);
            int instanceId = NetworkManager.Instance.Id;

            if (IncomingData.ContainsKey(curNode))
            {
                fieldInfo.SetValue(node, IncomingData[curNode]);
                DirtyRegistry.UpdateNode(curNode, fieldInfo.GetValue(node));
                IncomingData.Remove(curNode);
            }

            if (!DirtyRegistry.IsDirty(curNode, fieldInfo.GetValue(node))) return;

            if (owner != 0 && owner != instanceId)
                fieldInfo.SetValue(node, DirtyRegistry.PrevValues[curNode]);
            else
                DirtyQueue.Enqueue(PrimitiveSerializer.Serialize(fieldInfo.GetValue(node), syncAttribute.flags, iterators));

            DirtyRegistry.UpdateNode(curNode, fieldInfo.GetValue(node));
        }

        private static void SyncCollectionField(object node, FieldInfo fieldInfo, List<int> iterators, SyncAttribute syncAttribute, int owner)
        {
            if (!typeof(IList).IsAssignableFrom(fieldInfo.FieldType) && !fieldInfo.FieldType.IsArray)
                return;

            int instanceId = NetworkManager.Instance.Id;

            iterators.Add(0);

            IList list = (IList)fieldInfo.GetValue(node);

            for (int i = 0; i < list.Count; i++)
            {
                Node curNode = new(iterators);

                if (list[i].GetType().IsPrimitive || list[i].GetType().IsEnum || list[i] is string)
                {
                    if (IncomingData.ContainsKey(curNode))
                    {
                        list[i] = IncomingData[curNode];
                        DirtyRegistry.UpdateNode(curNode, list[i]);
                        IncomingData.Remove(curNode);
                    }

                    if (!DirtyRegistry.IsDirty(curNode, list[i])) return;

                    if (owner != 0 && owner != instanceId)
                        fieldInfo.SetValue(node, DirtyRegistry.PrevValues[curNode]);
                    else
                        DirtyQueue.Enqueue(PrimitiveSerializer.Serialize(list[i], syncAttribute.flags, iterators));
                }
                else
                {
                    Synchronize(list[i], iterators);
                }

                iterators[^1]++;
            }

            iterators.RemoveAt(iterators.Count - 1);
        }

        private static void SyncDictionaryField(object node, FieldInfo fieldInfo, List<int> iterators, SyncAttribute syncAttribute, int owner)
        {
            IDictionary dictionary = (IDictionary)fieldInfo.GetValue(node);

            if (dictionary == null)
                return;

            int instanceId = NetworkManager.Instance.Id;

            iterators.Add(0);
            iterators.Add(0);

            foreach (DictionaryEntry entry in dictionary)
            {
                iterators[^1] = 0;

                if (!entry.Key.GetType().IsPrimitive && entry.Key is not string)
                {
                    Synchronize(entry.Key, iterators);
                }

                iterators[^1] = 1;
                Node valueNode = new(iterators);

                if (entry.Value.GetType().IsPrimitive || entry.Value.GetType().IsEnum || entry.Value is string)
                {
                    if (IncomingData.ContainsKey(valueNode))
                    {
                        dictionary[entry.Key] = IncomingData[valueNode];
                        DirtyRegistry.UpdateNode(valueNode, dictionary[entry.Key]);
                        IncomingData.Remove(valueNode);
                    }

                    if (!DirtyRegistry.IsDirty(valueNode, dictionary[entry.Key])) return;

                    if (owner != 0 && owner != instanceId)
                        fieldInfo.SetValue(node, DirtyRegistry.PrevValues[valueNode]);
                    else
                        DirtyQueue.Enqueue(PrimitiveSerializer.Serialize(dictionary[entry.Key], syncAttribute.flags, iterators));
                }
                else
                {
                    Synchronize(dictionary[entry.Key], iterators);
                }

                iterators[^2]++;
            }

            iterators.RemoveAt(iterators.Count - 1);
            iterators.RemoveAt(iterators.Count - 1);
        }

        private static void SyncComplexField(object node, FieldInfo fieldInfo, List<int> iterators)
        {
            Synchronize(fieldInfo.GetValue(node), iterators);
        }

        #endregion
    }
}