using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NetworkScreen : MonoBehaviour
    {
        [SerializeField] protected InputField portInputField;
        [SerializeField] protected string defaultPort = "65432";

        public void ToggleNetworkScreen()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
