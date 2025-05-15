using Multiplayer.Network;
using Multiplayer.Utils;
using Objects;
using UnityEngine;
using Utils;

namespace Game
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private ObjectManager objectManager;

        private void Awake()
        {
            Timer.Start();
        }

        private void OnEnable()
        {
            NetworkManager.Instance.onConnectionEstablished += OnConnectionEstablished;
        }

        private void OnDisable()
        {
            NetworkManager.Instance.onConnectionEstablished -= OnConnectionEstablished;
        }

        private void Update()
        {
            NetworkManager.Instance.Update();
        }

        private void OnConnectionEstablished()
        {
            objectManager.RequestSpawn(0);
        }
    }
}