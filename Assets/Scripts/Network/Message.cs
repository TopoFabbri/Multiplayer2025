namespace Network
{
    public enum MessageType
    {
        HandShake = -1,
        Console,
        Position,
        SpawnRequest,
        Ping,
    }
    
    public abstract class Message<T>
    {
        protected MessageMetadata metadata;
        protected readonly T data;

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

        public MessageMetadata GetMessageMetadata() => metadata;
        public abstract byte[] Serialize();
        protected abstract T Deserialize(byte[] message);
        public T Deserialized() => data;
    }
}