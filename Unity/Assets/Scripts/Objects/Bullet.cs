using System.Collections;
using Interfaces;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using UnityEngine;

namespace Objects
{
    public class Bullet : SpawnableObject
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private int damage = 10;
        [SerializeField] private float lifeTime = 5;

        private bool OwnedByClient => Data.OwnerId == NetworkManager.Instance.Id;

        private void Start()
        {
            if (!OwnedByClient) return;
            
            StartCoroutine(DestroyAfterSeconds(lifeTime));
        }

        protected override void ApplyRotation()
        {
            transform.rotation = Quaternion.Euler(Data.Rot.Y, Data.Rot.X, 0f);
        }

        protected override void ApplyPosition()
        {
            transform.position = new Vector3(Data.Pos.x, Data.Pos.y, Data.Pos.z);
        }

        protected override void Update()
        {
            base.Update();

            if (!OwnedByClient) return;
            
            RequestMove();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!OwnedByClient) return;
            
            if (!other.TryGetComponent(out IDamageable damageable)) return;
            
            damageable.RequestHit(Data.Id, damage);
            NetworkManager.Instance.SendData(new NetDespawn(Data.Id).Serialize());
        }

        private void RequestMove()
        {
            Vector3 newPos = transform.position + transform.forward * (speed * Time.deltaTime);

            NetworkManager.Instance.SendData(new NetPosition(new Position(newPos.x, newPos.y, newPos.z, Data.Id)).Serialize());
        }
        
        private IEnumerator DestroyAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            
            NetworkManager.Instance.SendData(new NetDespawn(Data.Id).Serialize());
        }
    }
}