using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.Reflection;
using Multiplayer.Utils;
using Objects;
using UI;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Game
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private ClientNetworkScreen clientNetworkScreen;
        [SerializeField] private ChatScreen chatScreen;
        [SerializeField] private ColorPicker colorPicker;
        [SerializeField] private ObjectSpawner objectSpawner;

        [SerializeField] private bool authoritativeClient = true;
        
        
        [Sync] private GameModel gameModel;

        private ClientNetManager networkManager;

        protected override void Awake()
        {
            base.Awake();
            
            Timer.Start();
            networkManager = new ClientNetManager(authoritativeClient);
        }

        private void Start()
        {
            GameStateController.State = GameState.Connecting;
        }

        private void Update()
        {
            if (networkManager.IsInitiated)
                networkManager?.Update();
        }

        private void LateUpdate()
        {
            if (GameStateController.State == GameState.InGame)
                gameModel?.Update(); 
        }

        private void OnEnable()
        {
            networkManager.onConnectionEstablished += OnConnectionEstablishedServer;
            networkManager.Disconnected += OnDisconnectedHandler;
            InputListener.Disconnect += DisconnectHandler;
            networkManager.onConnectionEstablishedMatchMaker += ConnectHandler;
        }

        private void OnDisable()
        {
            networkManager.onConnectionEstablished -= OnConnectionEstablishedServer;
            networkManager.Disconnected -= OnDisconnectedHandler;
            InputListener.Disconnect -= DisconnectHandler;
            networkManager.onConnectionEstablishedMatchMaker -= ConnectHandler;
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
            
            networkManager.RequestDisconnect();
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