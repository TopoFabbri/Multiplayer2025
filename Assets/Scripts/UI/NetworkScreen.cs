using Network;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NetworkScreen : MonoBehaviour
    {
        [SerializeField] protected InputField portInputField;
        [SerializeField] private ChatScreen chatScreen;
        [SerializeField] protected string defaultPort = "65432";

        protected void SwitchToChatScreen()
        {
            chatScreen.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
