namespace Network
{
    public enum MessageType
    {
        HandShake = -1,
        Console = 0,
        Position = 1
    }

    public abstract class Message<T>
    {
        protected T data;

        protected Message(T data) => this.data = data;
        protected Message(byte[] data) => this.data = Deserialize(data);
        public abstract MessageType GetMessageType();
        public abstract byte[] Serialize();
        protected abstract T Deserialize(byte[] message);
        public T Deserialized() => data;
    }
}