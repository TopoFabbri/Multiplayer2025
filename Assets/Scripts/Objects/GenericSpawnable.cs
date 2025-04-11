using Network.Messages;

namespace Objects
{
    public class GenericSpawnable : SpawnableObject
    {
        private void Update()
        {
            if (transform.hasChanged)
                NetworkManager.Instance.SendData(new NetVector3(new Position(transform.position, ID)).Serialize());
        }
    }
}