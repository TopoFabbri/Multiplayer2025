using System;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Reflection
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcAttribute : Attribute
    {
        public Flags flags;
        
        public RpcAttribute(Flags flags = Flags.Checksum)
        {
            this.flags = flags;
        }
    }
}