using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class PrimitiveNetData
    {
        public List<int> path;
        public object data;
        
        public PrimitiveNetData(object data, List<int> path)
        {
            this.data = data;
            this.path = path;
        }
        
        public PrimitiveNetData()
        {
            path = new List<int>();
            data = null;
        }
    }
    
    public abstract class NetBasePrimitive : Message<PrimitiveNetData>
    {
        protected NetBasePrimitive(object data, Flags flags, List<int> path) : base(new PrimitiveNetData(data, path), flags)
        {
            metadata.Flags |= Flags.Primitive;
        }

        protected NetBasePrimitive(byte[] data) : base(data)
        {
        }
        
        protected byte[] SerializedPath()
        {
            List<byte> outData = new();

            outData.AddRange(BitConverter.GetBytes(data.path.Count));
            
            foreach (int index in data.path)
                outData.AddRange(BitConverter.GetBytes(index));
            
            return outData.ToArray();
        }

        protected static List<int> DeserializePath(byte[] message, ref int counter)
        {
            int pathCount = BitConverter.ToInt32(message, counter);
            counter += sizeof(int);

            List<int> path = new();

            for (int i = 0; i < pathCount; i++)
            {
                int index = BitConverter.ToInt32(message, counter);
                counter += sizeof(int);
                path.Add(index);
            }

            return path;
        }
    }
}