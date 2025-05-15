using CustomMath;
using Network.Messages;

namespace Objects
{
    public class GenericSpawnable : SpawnableObject
    {
        private void Update()
        {
            if (!transform.hasChanged) return;
            
            Vector3 pos = new(transform.position.x, transform.position.y, transform.position.z);
                
            NetworkManager.Instance.SendData(new NetPosition(new Position(pos, ID)).Serialize());
        }
    }
}