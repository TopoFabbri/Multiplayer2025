using System.Collections.Generic;
using System.Reflection;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Reflection
{
    public static class RpcRegistry
    {
        private static List<Node> Rpcs { get; } = new();
        private static HashSet<MethodBase> PatchedMethods { get; } = new();
        private static Dictionary<int, RpcMethods> RpcMethods { get; } = new();

        public static bool IsRpc(Node node)
        {
            if (Rpcs.Contains(node))
                return true;

            Rpcs.Add(node);
            
            return false;
        }
        
        public static bool IsMethodPatched(MethodBase method)
        {
            return !PatchedMethods.Add(method);
        }
        
        public static bool TryGetRpc(int objId, MethodBase method, out RpcMethods.RpcMethodInfo rpc)
        {
            if (RpcMethods.TryGetValue(objId, out RpcMethods rpcMethod))
                return rpcMethod.TryGetValue(method, out rpc);
            
            rpc = default;
            return false;
        }

        public static void AddRpc(int objId, MethodBase method, Node node, Flags flags)
        {
            if (!RpcMethods.ContainsKey(objId))
                RpcMethods.Add(objId, new RpcMethods());
            
            RpcMethods[objId].AddRpc(method, node, flags);
        }

        public static void RemoveRpc(int objId, MethodBase method)
        {
            RpcMethods.Remove(objId);
        }
    }
}