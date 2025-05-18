using System;
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
        [SerializeField] private float lifeTime;

        private bool OwnedByClient => Data.OwnerId == NetworkManager.Instance.Id;

        private void Start()
        {
            // StartCoroutine(DestroyAfterSeconds(lifeTime));
        }

        protected override void ApplyRotation()
        {
            transform.rotation = Quaternion.Euler(Data.Rot.Y, Data.Rot.X, 0f);
        }

        protected override void ApplyPosition()
        {
            transform.position = new UnityEngine.Vector3(Data.Pos.x, Data.Pos.y, Data.Pos.z);
        }

        protected override void Update()
        {
            base.Update();

            if (!OwnedByClient) return;
            
            RequestMove();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable damageable)) return;
            
            damageable.TakeDamage(damage);
        }

        private void RequestMove()
        {
            UnityEngine.Vector3 newPos = transform.position + transform.forward * (speed * Time.deltaTime);

            NetworkManager.Instance.SendData(new NetPosition(new Position(newPos.x, newPos.y, newPos.z, Data.Id)).Serialize());
        }
        
        private IEnumerator DestroyAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Destroy(gameObject);
        }
    }
}