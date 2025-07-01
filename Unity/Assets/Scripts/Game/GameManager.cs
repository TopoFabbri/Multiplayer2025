using System;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.NetworkFactory;
using Multiplayer.Reflection;
using Multiplayer.Utils;
using Objects;
using UI;
using UnityEditor;
using UnityEngine;
using Utils;
using Color = UnityEngine.Color;

namespace Game
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private ClientNetworkScreen clientNetworkScreen;
        [SerializeField] private ChatScreen chatScreen;
        [SerializeField] private ColorPicker colorPicker;
        [SerializeField] private ModelObjectManager modelObjectManager;
        
        [Sync] private GameModel gameModel;

        private ClientNetManager networkManager;

        protected override void Awake()
        {
            base.Awake();
            
            Timer.Start();
            networkManager = new ClientNetManager();
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
            networkManager.onConnectionEstablished += OnConnectionEstablished;
            networkManager.Disconnected += OnDisconnectedHandler;
            InputListener.Disconnect += DisconnectHandler;
            ClientNetworkScreen.Connect += ConnectHandler;
            ColorPicker.ColorPicked += OnColorPicked;
        }

        private void OnDisable()
        {
            networkManager.onConnectionEstablished -= OnConnectionEstablished;
            networkManager.Disconnected -= OnDisconnectedHandler;
            InputListener.Disconnect -= DisconnectHandler;
            ClientNetworkScreen.Connect -= ConnectHandler;
            ColorPicker.ColorPicked -= OnColorPicked;
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
            GameStateController.State = GameStateController.State == GameState.InGame ? GameState.ColorPick : GameState.Connecting;
        }

        private static void ConnectHandler(string name)
        {
            GameStateController.State = GameState.ColorPick;
        }

        private void OnColorPicked(Color color)
        {
            networkManager.Color = new Multiplayer.Network.Messages.Color(color.r, color.g, color.b, color.a);
            
            GameStateController.State = GameState.MatchMaking;

            networkManager.SendData(new NetReady(0).Serialize());
        }

        private void OnConnectionEstablished()
        {
            GameStateController.State = GameState.InGame;
            
            gameModel = new GameModel(modelObjectManager);
            
            SpawnableObjectData spawnableData = new()
            {
                OwnerId = NetworkManager.Instance.Id, PrefabId = 0
            };

            gameModel.RequestSpawn(spawnableData);
        }
    }
}