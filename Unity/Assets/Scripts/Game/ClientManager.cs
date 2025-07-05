using Multiplayer.Network;
using Multiplayer.Network.Messages;
using UI;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class ClientManager : GameManager
    {
        [SerializeField] private ClientNetworkScreen clientNetworkScreen;
        [SerializeField] private ChatScreen chatScreen;
        
        [SerializeField] private bool authoritativeClient = true;
        
        protected override void Awake()
        {
            base.Awake();
            
            networkManager = new ClientNetManager(authoritativeClient);
        }

        private void Start()
        {
            GameStateController.State = GameState.Connecting;
        }

        private void OnEnable()
        {
            networkManager.onConnectionEstablished += OnConnectionEstablishedServer;
            ((ClientNetManager)networkManager).Disconnected += OnDisconnectedHandler;
            InputListener.Disconnect += DisconnectHandler;
            ((ClientNetManager)networkManager).onConnectionEstablishedMatchMaker += ConnectHandler;
        }

        private void OnDisable()
        {
            networkManager.onConnectionEstablished -= OnConnectionEstablishedServer;
            ((ClientNetManager)networkManager).Disconnected -= OnDisconnectedHandler;
            InputListener.Disconnect -= DisconnectHandler;
            ((ClientNetManager)networkManager).onConnectionEstablishedMatchMaker -= ConnectHandler;
        }

        private void DisconnectHandler()
        {
            if (GameStateController.State == GameState.Connecting)
            {
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#else
                Application.Quit();
#endif
            }
            
            ((ClientNetManager)networkManager).RequestDisconnect();
        }

        private static void OnDisconnectedHandler()
        {
            GameStateController.State = GameStateController.State == GameState.InGame ? GameState.MatchMaking : GameState.Connecting;
        }

        private void ConnectHandler()
        {
            GameStateController.State = GameState.MatchMaking;

            networkManager.SendData(new NetReady(0).Serialize());
        }

        private void OnConnectionEstablishedServer()
        {
            gameModel = new GameModel(objectSpawner);
            
            GameStateController.State = GameState.InGame;
            
            Debug.Log(NetworkManager.Instance.Id);
        }
    }
}