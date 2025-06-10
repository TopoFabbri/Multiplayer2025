using System;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Reflection
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class SyncAttribute : Attribute
    {
        public Flags flags;
        
        public SyncAttribute(Flags flags = Flags.None)
        {
            this.flags = flags;
        }
    }
}