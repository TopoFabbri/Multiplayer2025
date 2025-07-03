using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class InputListener : MonoBehaviour
    {
        public static event Action Chat;
        public static event Action Disconnect;
        public static Action Click;
        public static event Action<Vector2> MousePos;

        private void OnMousePosition(InputValue input)
        {
            MousePos?.Invoke(input.Get<Vector2>());
        }
        
        private void OnChat()
        {
            Chat?.Invoke();
        }

        private void OnDisconnect()
        {
            Disconnect?.Invoke();
        }

        private void OnClick()
        {
            Click?.Invoke();
        }
    }
}
