using System.Net;

namespace Multiplayer.Network.interfaces
{
    public interface IReceiveData
    {
        public void OnReceiveData(byte[] data, IPEndPoint ipEndpoint);
    }
}