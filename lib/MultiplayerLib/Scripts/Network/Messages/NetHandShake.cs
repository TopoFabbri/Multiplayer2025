using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public struct HandShake
    {
        public readonly List<int> clients;
        public readonly uint randomSeed;
        public readonly bool fromServer;
        public HandShake(uint randomSeed, List<int> clients, bool fromServer)
        {
            this.fromServer = fromServer;
            this.randomSeed = randomSeed;
            this.clients = clients;
        }
    }
    
    public class NetHandShake : Message<HandShake>
    {
        public NetHandShake(HandShake data, bool important) : base(data)
        {
            metadata.Type = MessageType.HandShake;
            
            if (important)
                metadata.Flags = Flags.Important;
        }

        public NetHandShake(byte[] data) : base(data)
        {
        }

        protected override HandShake Deserialize(byte[] message)
        {
            bool fromServer = BitConverter.ToBoolean(message, MessageMetadata.Size);
            uint randomSeed  = BitConverter.ToUInt32(message, MessageMetadata.Size + sizeof(bool));
            
            List<int> clients = new();
            
            for (int i = MessageMetadata.Size + sizeof(int) + sizeof(bool); i < message.Length; i += 4)
            {
                byte[] curInt = { message[i], message[i + 1], message[i + 2], message[i + 3] };
                clients.Add(BitConverter.ToInt32(curInt, 0));
            }

            return new HandShake(randomSeed, clients, fromServer);
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            outData.AddRange(metadata.Serialize());

            outData.AddRange(BitConverter.GetBytes(data.fromServer));
            outData.AddRange(BitConverter.GetBytes(data.randomSeed));
            
            foreach (int i in data.clients)
                outData.AddRange(BitConverter.GetBytes(i));

            return outData.ToArray();
        }
    }
}