using System.Collections.Generic;
using System.Reflection;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Reflection
{
    public static class RpcRegistry
    {
        public struct RpcMethodInfo
        {
            public Node node;
            public Flags flags;
        }
        
        private static Dictionary<Node, bool> Rpcs { get; } = new();
        private static readonly Dictionary<MethodBase, RpcMethodInfo> RpcMethods = new();

        public static bool IsRpc(Node node)
        {
            if (Rpcs.TryGetValue(node, out bool isRpc)) 
                return isRpc;
            
            Rpcs.Add(node, isRpc);
            return true;
        }

        public static bool TryGetRpc(MethodBase method, out RpcMethodInfo rpc)
        {
            return RpcMethods.TryGetValue(method, out rpc);
        }
        
        public static void AddRpc(MethodBase method, Node node, Flags flags)
        {
            if (RpcMethods.ContainsKey(method))
                return;

            RpcMethods.Add(method, new RpcMethodInfo
            {
                node = node,
                flags = flags
            });
            
            Rpcs[node] = true;
        }
        
        public static void RemoveRpc(MethodBase method)
        {
            if (!RpcMethods.Remove(method, out RpcMethodInfo rpcMethodInfo))
                return;

            Rpcs[rpcMethodInfo.node] = false;
        }
    }
}