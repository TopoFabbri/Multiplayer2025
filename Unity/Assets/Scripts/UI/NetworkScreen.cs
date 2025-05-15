using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NetworkScreen : MonoBehaviour
    {
        [SerializeField] protected InputField portInputField;
        [SerializeField] private GameObject chatScreen;
        [SerializeField] protected string defaultPort = "65432";

        protected void SwitchToChatScreen()
        {
            if (chatScreen)
                chatScreen.SetActive(true);
            
            gameObject.SetActive(false);
        }
    }
}
