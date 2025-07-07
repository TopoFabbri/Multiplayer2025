using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.Primitives;
using Multiplayer.Utils;

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
        private static readonly Queue<byte[]> InvokedRpcs = new();
        private static readonly Dictionary<Node, object> IncomingData = new();
        private static readonly Dictionary<Node, ActionData> IncomingRpcs = new();
        
        public static object Synchronize(object node, List<int> iterators, int owner, int objectId)
        {
            iterators.Add(0);
            List<int> methodIterators = new();

            foreach (int iterator in iterators)
                methodIterators.Add(iterator);

            if (typeof(INetObject).IsAssignableFrom(node.GetType()))
            {
                INetObject netObject = (INetObject)node;
                owner = netObject.Owner;
                objectId = netObject.ObjectId;
            }
            

            Type nodeType = node.GetType();

            while (nodeType != typeof(object) && nodeType != null)
            {
                FieldInfo[] fields = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                foreach (FieldInfo fieldInfo in fields)
                {
                    SyncAttribute syncAttribute = fieldInfo.GetCustomAttribute<SyncAttribute>();

                    if (syncAttribute == null)
                        continue;

                    fieldInfo.SetValue(node ,SynchronizeNode(node, iterators, fieldInfo, syncAttribute, owner, objectId));

                    iterators[^1]++;
                }

                MethodInfo[] methods = nodeType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                foreach (MethodInfo methodInfo in methods)
                {
                    RpcAttribute rpcAttribute = methodInfo.GetCustomAttribute<RpcAttribute>();

                    Node methodNode = new(methodIterators);

                    if (rpcAttribute == null || methodInfo.GetParameters().Length != 0 || methodInfo.ReturnType != typeof(void))
                        continue;

                    if (RpcRegistry.IsRpc(methodNode))
                    {
                        if (IncomingRpcs.ContainsKey(methodNode) && IncomingRpcs[methodNode].action == methodInfo.Name)
                        {
                            methodInfo.Invoke(node, new object[] { });

                            IncomingRpcs.Remove(methodNode);
                        }

                        continue;
                    }

                    if (objectId < 0) continue;
                    
                    MakeRpc(objectId, methodInfo, methodNode, rpcAttribute);

                    methodIterators[^1]++;
                }

                nodeType = nodeType.BaseType;
            }

            iterators.RemoveAt(iterators.Count - 1);
            methodIterators.RemoveAt(methodIterators.Count - 1);
            
            return node;
        }

        private static object SynchronizeNode(object node, List<int> iterators, FieldInfo fieldInfo, SyncAttribute syncAttribute, int owner, int objectId)
        {
            if (fieldInfo.FieldType.IsPrimitive || fieldInfo.FieldType.IsEnum)
                SyncPrimitiveField(node, fieldInfo, iterators, syncAttribute, owner);
            else if (typeof(IDictionary).IsAssignableFrom(fieldInfo.FieldType))
                SyncDictionaryField(node, fieldInfo, iterators, syncAttribute, owner, objectId);
            else if (fieldInfo.FieldType != typeof(string) && (fieldInfo.FieldType.IsArray || typeof(ICollection).IsAssignableFrom(fieldInfo.FieldType)))
                SyncCollectionField(node, fieldInfo, iterators, syncAttribute, owner, objectId);
            else
                fieldInfo.SetValue(node, Synchronize(fieldInfo.GetValue(node), iterators, owner, objectId));
            
            return fieldInfo.GetValue(node);
        }

        #region Sync Handlers

        private static void SyncPrimitiveField(object node, FieldInfo fieldInfo, List<int> iterators, SyncAttribute syncAttribute, int owner)
        {
            Node curNode = new(iterators);

            if (IncomingData.ContainsKey(curNode))
            {
                fieldInfo.SetValue(node, IncomingData[curNode]);
                DirtyRegistry.UpdateNode(curNode, fieldInfo.GetValue(node));
                IncomingData.Remove(curNode);
            }

            if (!DirtyRegistry.IsDirty(curNode, fieldInfo.GetValue(node))) return;

            if ((!OwnedByThis(owner) || !IsAuthoritativeClient()) && !IsServer())
                fieldInfo.SetValue(node, DirtyRegistry.PrevValues[curNode]);
            else
                DirtyQueue.Enqueue(PrimitiveSerializer.Serialize(fieldInfo.GetValue(node), syncAttribute.flags, iterators));

            DirtyRegistry.UpdateNode(curNode, fieldInfo.GetValue(node));
        }

        private static void SyncCollectionField(object node, FieldInfo fieldInfo, List<int> iterators, SyncAttribute syncAttribute, int owner, int objectId)
        {
            if (!typeof(IList).IsAssignableFrom(fieldInfo.FieldType) && !fieldInfo.FieldType.IsArray)
                return;

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

                    if ((!OwnedByThis(owner) || !IsAuthoritativeClient()) && !IsServer())
                        fieldInfo.SetValue(node, DirtyRegistry.PrevValues[curNode]);
                    else
                        DirtyQueue.Enqueue(PrimitiveSerializer.Serialize(list[i], syncAttribute.flags, iterators));
                }
                else
                {
                    list[i] = Synchronize(list[i], iterators, owner, objectId);
                }

                iterators[^1]++;
            }

            iterators.RemoveAt(iterators.Count - 1);
        }

        private static void SyncDictionaryField(object node, FieldInfo fieldInfo, List<int> iterators, SyncAttribute syncAttribute, int owner, int objectId)
        {
            IDictionary dictionary = (IDictionary)fieldInfo.GetValue(node);

            if (dictionary == null)
                return;

            iterators.Add(0);
            iterators.Add(0);
            
            List<object> keys = new();
            
            foreach (DictionaryEntry entry in dictionary)
                keys.Add(entry.Key);
            
            foreach (object key in keys)
            {
                iterators[^1] = 0;

                if (!key.GetType().IsPrimitive && key is not string)
                {
                    Synchronize(key, iterators, owner, objectId);
                }

                iterators[^1] = 1;
                Node valueNode = new(iterators);

                if (dictionary[key].GetType().IsPrimitive || dictionary[key].GetType().IsEnum || dictionary[key] is string)
                {
                    if (IncomingData.ContainsKey(valueNode))
                    {
                        dictionary[key] = IncomingData[valueNode];
                        DirtyRegistry.UpdateNode(valueNode, dictionary[key]);
                        IncomingData.Remove(valueNode);
                    }

                    if (!DirtyRegistry.IsDirty(valueNode, dictionary[key])) return;

                    if ((!OwnedByThis(owner) || !IsAuthoritativeClient()) && !IsServer())
                        fieldInfo.SetValue(node, DirtyRegistry.PrevValues[valueNode]);
                    else
                        DirtyQueue.Enqueue(PrimitiveSerializer.Serialize(dictionary[key], syncAttribute.flags, iterators));
                }
                else
                {
                    dictionary[key] = Synchronize(dictionary[key], iterators, owner, objectId);
                }

                iterators[^2]++;
            }

            iterators.RemoveAt(iterators.Count - 1);
            iterators.RemoveAt(iterators.Count - 1);
        }

        #endregion

        #region Rpc

        private static void MakeRpc(int objId, MethodInfo methodInfo, Node methodNode, RpcAttribute rpcAttribute)
        {
            RpcRegistry.AddRpc(objId, methodInfo, methodNode, rpcAttribute.flags);

            if (RpcRegistry.IsMethodPatched(methodInfo))
                return;
            
            Harmony harmony = new("RPC");
            HarmonyMethod postfix = new(typeof(Synchronizer).GetMethod(nameof(MethodHookPostfix)));

            string path = string.Join("-", methodNode.Path);

            Log.Write("Patching: " + methodInfo.Name + " at: " + path);
            Log.NewLine();

            harmony.Patch(methodInfo, null, postfix);
        }

        // ReSharper disable InconsistentNaming
        public static void MethodHookPostfix(MethodBase __originalMethod, object __instance)
        {
            if (__instance is not INetObject netObject) return;

            if ((!OwnedByThis(netObject.Owner) || !IsAuthoritativeClient()) && !IsServer()) return;

            if (!RpcRegistry.TryGetRpc(netObject.ObjectId, __originalMethod, out RpcMethods.RpcMethodInfo rpc)) return;

            NetAction netAction = new(new ActionData(rpc.node, __originalMethod.Name), rpc.flags);

            InvokedRpcs.Enqueue(netAction.Serialize());
        }

        public static byte[] DequeueDirty()
        {
            return DirtyQueue.Count == 0 ? null : DirtyQueue.Dequeue();
        }

        #endregion

        #region Utils

        public static byte[] DequeueRpc()
        {
            return InvokedRpcs.Count == 0 ? null : InvokedRpcs.Dequeue();
        }

        public static void AddIncomingData(List<int> key, object data)
        {
            Node keyNode = new(key);

            IncomingData[keyNode] = data;
        }

        public static void AddIncomingRpc(ActionData actionData)
        {
            if (!IncomingRpcs.TryAdd(actionData.node, actionData))
                IncomingRpcs[actionData.node] = actionData;
        }

        public static bool HasDirty()
        {
            return DirtyQueue.Count > 0;
        }

        public static bool HasRpc()
        {
            return InvokedRpcs.Count > 0;
        }
        
        private static bool IsServer()
        {
            return NetworkManager.Instance is ServerNetManager;
        }

        private static bool OwnedByThis(int owner)
        {
            return NetworkManager.Instance.Id == owner || NetworkManager.Instance.Id == 0;
        }

        private static bool IsAuthoritativeClient()
        {
            if (NetworkManager.Instance is ClientNetManager clientNetManager)
                return clientNetManager.IsAuthoritative;

            return false;
        }

        #endregion
    }
}