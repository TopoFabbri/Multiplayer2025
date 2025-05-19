using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.NetworkFactory;
using Multiplayer.Utils;
using Objects;
using UI;
using UnityEngine;
using Utils;
using Color = UnityEngine.Color;

namespace Game
{
    public enum GameState
    {
        Connecting,
        ColorPick,
        MatchMaking,
        InGame
    }
    
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private ClientNetworkScreen clientNetworkScreen;
        [SerializeField] private ChatScreen chatScreen;
        [SerializeField] private ColorPicker colorPicker;
        
        private ClientNetManager networkManager;
        public static GameState State { get; private set; }

        private void Awake()
        {
            Timer.Start();
            networkManager = new ClientNetManager();
            
            State = GameState.Connecting;
        }

        private void Update()
        {
            if (networkManager.IsInitiated)
                networkManager?.Update();
        }

        private void OnEnable()
        {
            networkManager.onConnectionEstablished += OnConnectionEstablished;
            InputListener.Disconnect += DisconnectHandler;
            ClientNetworkScreen.Connect += ConnectHandler;
            ColorPicker.ColorPicked += OnColorPicked;
        }

        private void OnDisable()
        {
            networkManager.onConnectionEstablished -= OnConnectionEstablished;
            InputListener.Disconnect -= DisconnectHandler;
            ClientNetworkScreen.Connect -= ConnectHandler;
            ColorPicker.ColorPicked -= OnColorPicked;
        }

        private void DisconnectHandler()
        {
            networkManager.RequestDisconnect();

            if (State == GameState.InGame)
            {
                State = GameState.MatchMaking;
                ObjectManager.Instance.Disconnect();
            }
            else
            {
                State = GameState.Connecting;
                clientNetworkScreen.ToggleNetworkScreen();
            }
        }

        private void ConnectHandler()
        {
            State = GameState.ColorPick;
            
            colorPicker.SetActive(true);
        }

        private void OnColorPicked(Color color)
        {
            networkManager.Color = new Multiplayer.Network.Messages.Color(color.r, color.g, color.b, color.a);
            
            colorPicker.SetActive(false);
            State = GameState.MatchMaking;
            
            networkManager.SendData(new NetReady(0).Serialize());
        }
        
        private void OnConnectionEstablished()
        {
            State = GameState.InGame;
            chatScreen.gameObject.SetActive(true);
            
            SpawnableObjectData spawnableData = new()
            {
                OwnerId = NetworkManager.Instance.Id,
                PrefabId = 0,
                Pos = Multiplayer.CustomMath.Vector3.Zero,
                Rot = System.Numerics.Vector2.Zero
            };
            
            
            ObjectManager.Instance.RequestSpawn(spawnableData);
        }
    }
}