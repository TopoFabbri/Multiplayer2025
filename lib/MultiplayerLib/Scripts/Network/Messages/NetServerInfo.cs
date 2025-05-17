using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public struct ServerInfo
    {
        public int port;

        public ServerInfo(int port)
        {
            this.port = port;
        }
    }
    
    public class NetServerInfo : Message<ServerInfo>
    {
        
        public NetServerInfo(ServerInfo data) : base(data)
        {
            Metadata.Type = MessageType.ServerInfo;
            Metadata.Flags = Flags.Important | Flags.Checksum;
        }
        
        public NetServerInfo(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(Metadata.Serialize());
            
            outData.AddRange(BitConverter.GetBytes(data.port));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override ServerInfo Deserialize(byte[] message)
        {
            ServerInfo svInfo = new(BitConverter.ToInt32(message, MessageMetadata.Size));
                
            return svInfo;
        }
    }
}