using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network
{
    public static class MessageSender
    {
        private static NetworkManager _netManager;

        public static void Send(byte[] data)
        {
            if (CanSend(data))
                _netManager.SendData(data);
        }

        private static bool CanSend(byte[] data)
        {
            _netManager ??= NetworkManager.Instance;

            if (_netManager == null)
                return false;

            if (_netManager is not ClientNetManager clientNetManager)
                return true;
            
            if (clientNetManager.IsAuthoritative)
                return true;

            MessageMetadata metadata = MessageMetadata.Deserialize(data);

            return !metadata.IsPrimitive && metadata.Type != MessageType.Action;
        }
    }
}