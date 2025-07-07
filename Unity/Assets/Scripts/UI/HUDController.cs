using System.Collections.Generic;
using Game;
using Game.GameBoard;
using Multiplayer.Network;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pingText;
        [SerializeField] private TextMeshProUGUI matchMakerText;
        [SerializeField] private TextMeshProUGUI gameText;
        [SerializeField] private TextMeshProUGUI movesText;
        
        private void Start()
        {
            GameStateController.StateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            GameStateController.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Connecting:
                    pingText.gameObject.SetActive(false);
                    matchMakerText.transform.parent.gameObject.SetActive(false);
                    break;

                case GameState.MatchMaking:
                    pingText.gameObject.SetActive(true);
                    matchMakerText.transform.parent.gameObject.SetActive(true);
                    break;

                case GameState.InGame:
                    pingText.gameObject.SetActive(true);
                    matchMakerText.transform.parent.gameObject.SetActive(false);
                    break;

                default:
                    pingText.gameObject.SetActive(false);
                    matchMakerText.transform.parent.gameObject.SetActive(false);
                    break;
            }
        }

        private void Update()
        {
            pingText.text = "";
            
            foreach (KeyValuePair<int, float> pingById in ((ClientNetManager)NetworkManager.Instance).PingsByClientId)
                pingText.text += ((ClientNetManager)NetworkManager.Instance).GetName(pingById.Key) + ": " + (int)(pingById.Value * 1000) + "ms\n";
            
            gameText.text = Board.GameStateText;
            movesText.text = "Moves: " + (10 - GameModel.Moves);
        }
    }
}