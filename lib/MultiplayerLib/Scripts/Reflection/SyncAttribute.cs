using System;

namespace Multiplayer.Reflection
{
    [Flags]
    public enum Flags
    {
        None = 0,
        Sortable = 1,
        Important = 2,
        Critical = 4,
        Checksum = 8
    }

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