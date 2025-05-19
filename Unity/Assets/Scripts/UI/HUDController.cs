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

        private void Update()
        {
            int ping = (int)(NetworkManager.Instance.Ping * 1000f);

            pingText.text = $"{ping}ms";

            switch (GameManager.State)
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
    }
}