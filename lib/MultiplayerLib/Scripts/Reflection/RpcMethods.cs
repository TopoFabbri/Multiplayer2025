using System.Collections.Generic;
using System.Reflection;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Reflection
{
    public class RpcMethods
    {
        public struct RpcMethodInfo
        {
            public Node node;
            public Flags flags;
        }

        private readonly Dictionary<MethodBase, RpcMethodInfo> rpcMethodInfos = new();

        public bool TryGetValue(MethodBase methodBase, out RpcMethodInfo rpcInfo)
        {
            return rpcMethodInfos.TryGetValue(methodBase, out rpcInfo);
        }

        public void AddRpc(MethodBase method, Node node, Flags flags)
        {
            if (rpcMethodInfos.ContainsKey(method))
                rpcMethodInfos[method] = new RpcMethodInfo { node = node, flags = flags };
            
            rpcMethodInfos.Add(method, new RpcMethodInfo { node = node, flags = flags });
        }

        public void RemoveRpc(MethodBase method)
        {
            rpcMethodInfos.Remove(method);
        }
    }
}