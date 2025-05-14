using System;
using System.Collections.Generic;

namespace Network.Messages
{
    public class MessageMetadata
    {
        public MessageType Type { get; set; }
        public int Id { get; set; }
        public bool Important { get; set; }
        public int SenderId { get; set; }

        public byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(BitConverter.GetBytes((int)Type));
            outData.AddRange(BitConverter.GetBytes(Id));
            outData.AddRange(BitConverter.GetBytes(SenderId));
            outData.AddRange(BitConverter.GetBytes(Important));
            
            return outData.ToArray();
        }
        
        public static MessageMetadata Deserialize(byte[] message)
        {
            MessageMetadata outData = new();
            
            outData.Type = (MessageType) BitConverter.ToInt32(message, 0);
            outData.Id = BitConverter.ToInt32(message, sizeof(int));
            outData.SenderId = BitConverter.ToInt32(message, sizeof(int) * 2);
            outData.Important = BitConverter.ToBoolean(message, sizeof(int) * 3);
            
            return outData; 
        }
        
        public static int Size
        {
            get
            {
                int size = sizeof(MessageType);
                size += sizeof(int);
                size += sizeof(int);
                size += sizeof(bool);
                
                return size; 
            }
        }
    }

    public class CheckSum
    {
        public uint CheckSum1 { get; set; }
        public uint CheckSum2 { get; set; }
        public bool Corrupted { get; private set; }

        public CheckSum(byte[] data)
        {
            
        }
    }
}