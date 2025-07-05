using Multiplayer.Network;
using Multiplayer.Reflection;
using Multiplayer.Utils;
using Objects;
using UnityEngine;
using Utils;

namespace Game
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] protected ObjectSpawner objectSpawner;
        
        [Sync] protected GameModel gameModel;

        protected NetworkManager networkManager;

        protected override void Awake()
        {
            base.Awake();
            
            Timer.Start();
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
    }
}