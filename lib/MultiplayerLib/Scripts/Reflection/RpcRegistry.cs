using System.Collections.Generic;
using System.Reflection;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Reflection
{
    public static class RpcRegistry
    {
        private static List<Node> Rpcs { get; } = new();

        private static Dictionary<int, RpcMethods> RpcMethods { get; } = new();

        public static bool IsRpc(Node node)
        {
            if (Rpcs.Contains(node))
                return true;

            Rpcs.Add(node);
            return false;
        }

        public static bool TryGetRpc(int owner, MethodBase method, out RpcMethods.RpcMethodInfo rpc)
        {
            if (RpcMethods.TryGetValue(owner, out RpcMethods rpcMethod))
                return rpcMethod.TryGetValue(method, out rpc);
            
            rpc = default;
            return false;
        }

        public static void AddRpc(int owner, MethodBase method, Node node, Flags flags)
        {
            if (!RpcMethods.ContainsKey(owner))
                RpcMethods.Add(owner, new RpcMethods());
            
            RpcMethods[owner].AddRpc(method, node, flags);
        }

        public static void RemoveRpc(int owner, MethodBase method)
        {
            RpcMethods.Remove(owner);
        }
    }
}