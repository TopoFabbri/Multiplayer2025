﻿using System;
using System.Collections.Generic;

namespace Network.Messages
{
    public class MessageMetadata
    {
        public MessageType Type { get; set; }
        public int Id { get; set; }
        public bool Received { get; set; }
        public bool Important { get; set; }
        public bool Numbered { get; set; }

        public byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(BitConverter.GetBytes((int)Type));
            outData.AddRange(BitConverter.GetBytes(Id));
            outData.AddRange(BitConverter.GetBytes(Received));
            outData.AddRange(BitConverter.GetBytes(Important));
            outData.AddRange(BitConverter.GetBytes(Numbered));
            
            return outData.ToArray();
        }
        
        public static MessageMetadata Deserialize(byte[] message)
        {
            return new MessageMetadata()
            {
                Type = (MessageType) BitConverter.ToInt32(message, 0),
                Id = BitConverter.ToInt32(message, sizeof(int)),
                Received = BitConverter.ToBoolean(message, sizeof(int) * 2),
                Important = BitConverter.ToBoolean(message, sizeof(int) * 3),
                Numbered = BitConverter.ToBoolean(message, sizeof(int) * 4)
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