using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NetworkScreen : MonoBehaviour
    {
        [SerializeField] protected InputField portInputField;
        [SerializeField] private ChatScreen chatScreen;

        protected void SwitchToChatScreen()
        {
            ChatScreen.Instance.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
