using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;
using Multiplayer.Reflection;

namespace Multiplayer.Network.Messages
{

    public struct ActionData
    {
        public Node node;
        public string action;
        
        public ActionData(Node node, string action)
        {
            this.node = node;
            this.action = action;
        }

        public readonly byte[] Serialize()
        {
            List<byte> data = new();
            
            data.AddRange(BitConverter.GetBytes(node.Path.Count));
            
            foreach (int n in node.Path)
                data.AddRange(BitConverter.GetBytes(n));
            
            data.AddRange(BitConverter.GetBytes(action.Length));
            data.AddRange(System.Text.Encoding.UTF8.GetBytes(action));
            
            return data.ToArray();
        }
        
        public static ActionData Deserialize(byte[] data, ref int counter)
        {
            int count = BitConverter.ToInt32(data, counter);
            counter += sizeof(int);
            
            Node node = new(new List<int>());
            for (int i = 0; i < count; i++)
            {
                node.Path.Add(BitConverter.ToInt32(data, counter));
                counter += sizeof(int);
            }
            
            int actionLength = BitConverter.ToInt32(data, counter);
            counter += sizeof(int);
            
            string action = System.Text.Encoding.UTF8.GetString(data, counter, actionLength);
            
            return new ActionData(node, action);
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