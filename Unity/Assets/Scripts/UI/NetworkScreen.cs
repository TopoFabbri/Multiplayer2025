using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NetworkScreen : MonoBehaviour
    {
        [SerializeField] protected InputField portInputField;
        [SerializeField] protected string defaultPort = "65432";

        protected virtual void Awake()
        {
            GameStateController.StateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            GameStateController.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState newState)
        {
            gameObject.SetActive(newState == GameState.Connecting);
        }
    }
}
