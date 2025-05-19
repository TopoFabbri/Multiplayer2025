using System.Collections.Generic;
using Game;
using Multiplayer.Network;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pingText;
        [SerializeField] private TextMeshProUGUI matchMakerText;

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
                    matchMakerText.gameObject.SetActive(false);
                    break;

                case GameState.MatchMaking:
                    pingText.gameObject.SetActive(true);
                    matchMakerText.gameObject.SetActive(true);
                    break;

                case GameState.ColorPick:
                case GameState.InGame:
                    pingText.gameObject.SetActive(true);
                    matchMakerText.gameObject.SetActive(false);
                    break;

                default:
                    pingText.gameObject.SetActive(false);
                    matchMakerText.gameObject.SetActive(false);
                    break;
            }
        }

        private void Update()
        {
            pingText.text = "";
            
            foreach (KeyValuePair<int, float> pingById in ((ClientNetManager)NetworkManager.Instance).PingsByClientId)
                pingText.text += "Client " + pingById.Key + ": " + (int)(pingById.Value * 1000) + "ms\n";
        }
    }
}