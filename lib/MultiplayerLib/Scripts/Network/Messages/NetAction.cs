using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{

    public struct ActionData
    {
        public List<int> node;
        public string action;
        
        public ActionData(List<int> node, string action)
        {
            this.node = node;
            this.action = action;
        }

        public readonly byte[] Serialize()
        {
            List<byte> data = new();
            
            data.AddRange(BitConverter.GetBytes(node.Count));
            
            foreach (int n in node)
                data.AddRange(BitConverter.GetBytes(n));
            
            data.AddRange(BitConverter.GetBytes(action.Length));
            data.AddRange(System.Text.Encoding.UTF8.GetBytes(action));
            
            return data.ToArray();
        }
        
        public static ActionData Deserialize(byte[] data, ref int counter)
        {
            int count = BitConverter.ToInt32(data, counter);
            counter += sizeof(int);
            
            List<int> nodeList = new();
            for (int i = 0; i < count; i++)
            {
                nodeList.Add(BitConverter.ToInt32(data, counter));
                counter += sizeof(int);
            }
            
            int actionLength = BitConverter.ToInt32(data, counter);
            counter += sizeof(int);
            
            string action = System.Text.Encoding.UTF8.GetString(data, counter, actionLength);
            
            return new ActionData(nodeList, action);
        }
    }
    
    public class NetAction : Message<ActionData>
    {
        public NetAction(ActionData data, Flags flags) : base(data)
        {
            Metadata.Type = MessageType.Action;
            Metadata.Flags = flags;
        }

        public NetAction(byte[] data) : base(data) { }

        public override byte[] Serialize()
        {
            List<byte> serializedData = new();
            
            serializedData.AddRange(BitConverter.GetBytes((int)MessageType.Action));
            serializedData.AddRange(metadata.Serialize());
            serializedData.AddRange(data.Serialize());
            serializedData.AddRange(GetCheckSum(serializedData));
            
            return serializedData.ToArray();
        }

        protected override ActionData Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            ActionData actionData = ActionData.Deserialize(message, ref counter);
            
            return actionData;
        }
    }
}