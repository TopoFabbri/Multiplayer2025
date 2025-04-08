using Network.Messages;
using Objects;
using UnityEngine;
using Utils;

namespace Game
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private ObjectManager objectManager;

        private void OnEnable()
        {
            NetworkManager.Instance.onConnectionEstablished += OnConnectionEstablished;
        }
        
        private void OnDisable()
        {
            if (NetworkManager.Instance)
                NetworkManager.Instance.onConnectionEstablished -= OnConnectionEstablished;
        }

        private void OnConnectionEstablished()
        {
            objectManager.RequestSpawn(0);
        }
    }
}
