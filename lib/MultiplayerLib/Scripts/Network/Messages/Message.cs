using System;
using System.Collections.Generic;
using System.Linq;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public enum MessageType
    {
        HandShake = -1,
        Console,
        SpawnRequest,
        Ping,
        Disconnect,
        Acknowledge,
        ServerInfo,
        Crouch,
        Jump,
        Despawn,
        Ready,
        Int,
        Byte,
        Bool,
        Float,
        Double,
        Long,
        Short,
        Char,
        String,
        UShort,
        UInt,
        ULong,
        Action
    }

    public abstract class Message<T> : ISerializable
    {
        protected readonly MessageMetadata metadata;
        protected readonly T data;

        public MessageMetadata Metadata => metadata;
        public T Data => data;
        
        protected Message(T data)
        {
            this.data = data;
            metadata = new MessageMetadata();
            Metadata.SenderId = NetworkManager.Instance.Id;
        }
        
        protected Message(T data, Flags flags)
        {
            this.data = data;
            metadata = new MessageMetadata
            {
                Flags = flags,
                SenderId = NetworkManager.Instance.Id
            };
        }

        protected Message(byte[] data)
        {
            metadata = MessageMetadata.Deserialize(data);
            this.data = Deserialize(data);
        }
        public abstract byte[] Serialize();
        protected byte[] GetCheckSum(List<byte> data)
        {
            List<byte> checksum = new();
            List<byte> dataCopy = data.ToList();
            
            uint cs1 = CheckSum.Get(dataCopy.ToArray(), true);
            dataCopy.AddRange(BitConverter.GetBytes(cs1));
            
            uint cs2 = CheckSum.Get(dataCopy.ToArray(), false);
            
            checksum.AddRange(BitConverter.GetBytes(cs1));
            checksum.AddRange(BitConverter.GetBytes(cs2));
            
            return checksum.ToArray();
        }
        
        protected abstract T Deserialize(byte[] message);
        public T Deserialized() => data;
    }
}