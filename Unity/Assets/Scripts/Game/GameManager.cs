using System;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.Utils;
using Objects;
using UnityEngine;
using Utils;

namespace Game
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private ObjectManager objectManager;
        
        private ClientNetManager networkManager;

        private void Awake()
        {
            Timer.Start();
            networkManager = new ClientNetManager();
        }

        private void Update()
        {
            if (networkManager.IsInitiated)
                networkManager?.Update();
        }

        private void OnEnable()
        {
            NetworkManager.Instance.onConnectionEstablished += OnConnectionEstablished;
        }

        private void OnDisable()
        {
            NetworkManager.Instance.onConnectionEstablished -= OnConnectionEstablished;
        }
        
        private void OnConnectionEstablished()
        {
            objectManager.RequestSpawn(0);
        }
    }
}