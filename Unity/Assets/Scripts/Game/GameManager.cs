using Multiplayer.Network;
using Multiplayer.NetworkFactory;
using Multiplayer.Utils;
using Objects;
using UI;
using UnityEngine;
using Utils;

namespace Game
{
    public enum GameState
    {
        Connecting,
        MatchMaking,
        InGame
    }
    
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private ClientNetworkScreen clientNetworkScreen;
        [SerializeField] private ChatScreen chatScreen;
        
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
        }

        private void OnDisable()
        {
            networkManager.onConnectionEstablished -= OnConnectionEstablished;
            InputListener.Disconnect -= DisconnectHandler;
            ClientNetworkScreen.Connect -= ConnectHandler;
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

        private static void ConnectHandler()
        {
            State = GameState.MatchMaking;
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