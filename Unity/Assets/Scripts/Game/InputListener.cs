using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class InputListener : MonoBehaviour
    {
        public static event Action<Vector2> Move;
        public static event Action<Vector2> Look;
        public static event Action Crouch;
        public static event Action Jump;
        public static event Action Chat;
        public static event Action Disconnect;
        public static event Action Shoot;

        private void OnMove(InputValue input)
        {
            Move?.Invoke(input.Get<Vector2>());
        }
        
        private void OnLook(InputValue input)
        {
            Look?.Invoke(input.Get<Vector2>());
        }

        private void OnCrouch()
        {
            Crouch?.Invoke();
        }
        
        private void OnJump()
        {
            Jump?.Invoke();
        }
        
        private void OnChat()
        {
            Chat?.Invoke();
        }

        private void OnDisconnect()
        {
            Disconnect?.Invoke();
        }

        private void OnShoot()
        {
            Shoot?.Invoke();
        }
    }
}
