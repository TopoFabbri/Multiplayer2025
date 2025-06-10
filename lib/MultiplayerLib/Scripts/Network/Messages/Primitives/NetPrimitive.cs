using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class Primitive<T>
    {
        public List<int> path;
        public T data;
        
        public Primitive(T data, List<int> path)
        {
            this.data = data;
            this.path = path;
        }
        
        public Primitive()
        {
            path = new List<int>();
            data = default;
        }
    }
    
    public abstract class NetPrimitive<T> : Message<Primitive<T>>
    {
        protected NetPrimitive(T data, Flags flags, List<int> path) : base(new Primitive<T>(data, path), flags)
        {
        }

        protected NetPrimitive(byte[] data) : base(data)
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

        protected List<int> DeserializePath(byte[] message, ref int counter)
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