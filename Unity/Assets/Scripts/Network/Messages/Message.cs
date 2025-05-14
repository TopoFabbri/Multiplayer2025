namespace Network.Messages
{
    public enum MessageType
    {
        HandShake = -1,
        Console,
        Position,
        SpawnRequest,
        Ping,
        Disconnect,
        Acknowledge
    }
    
    public abstract class Message<T>
    {
        protected readonly MessageMetadata metadata;
        protected readonly T data;
        protected readonly CheckSum checksum;

        public MessageMetadata Metadata => metadata;
        public T Data => data;
        public bool Corrupted => checksum.Corrupted;
        
        protected Message(T data)
        {
            this.data = data;
            metadata = new MessageMetadata();
        }

        protected Message(byte[] data)
        {
            metadata = MessageMetadata.Deserialize(data);
            this.data = Deserialize(data);
        }
        public abstract byte[] Serialize();
        protected abstract T Deserialize(byte[] message);
        public T Deserialized() => data;
    }
}