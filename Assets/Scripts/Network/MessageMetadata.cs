using System;
using System.Collections.Generic;

namespace Network
{
    public class MessageMetadata
    {
        public MessageType Type { get; set; }

        public byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(BitConverter.GetBytes((int)Type));
            
            return outData.ToArray();
        }
        
        public static MessageMetadata Deserialize(byte[] message)
        {
            return new MessageMetadata()
            {
                Type = (MessageType) BitConverter.ToInt32(message, 0)
            };
        }
        
        public static int Size
        {
            get
            {
                return sizeof(MessageType);
            }

        }
    }
}