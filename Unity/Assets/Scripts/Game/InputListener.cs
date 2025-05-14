using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class InputListener : MonoBehaviour
    {
        public static event Action<Vector2> Move;
        public static event Action<Vector2> Look;
        public static event Action Chat;

        private void OnMove(InputValue input)
        {
            Move?.Invoke(input.Get<Vector2>());
        }
        
        private void OnLook(InputValue input)
        {
            Look?.Invoke(input.Get<Vector2>());
        }

        private void OnChat()
        {
            Chat?.Invoke();
        }
    }
}
