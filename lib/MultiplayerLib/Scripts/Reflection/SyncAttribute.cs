using System;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Reflection
{

    [AttributeUsage(AttributeTargets.Field)]
    public class SyncAttribute : Attribute
    {
        public readonly Flags flags;
        
        public SyncAttribute(Flags flags = Flags.Checksum)
        {
            this.flags = flags;
        }
    }
}