using Interfaces;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using UnityEngine;

namespace Objects
{
    public class DamageCaster : MonoBehaviour, IDamageable
    {
        public int Id { get; set; }

        public void RequestHit(int bulletId, int damage)
        {
            Hit hit = new(Id, bulletId, damage);

            NetworkManager.Instance.SendData(new NetHit(hit).Serialize());
        }
    }
}