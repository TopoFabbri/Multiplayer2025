using Multiplayer.Network;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pingText;

        private void Update()
        {
            int ping = (int)(NetworkManager.Instance.Ping * 1000f);
            
            pingText.text = $"{ping}ms";
        }
    }
}
