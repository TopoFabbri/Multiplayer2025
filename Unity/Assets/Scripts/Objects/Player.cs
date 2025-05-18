using Game;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.NetworkFactory;
using UnityEngine;

namespace Objects
{
    public class Player : SpawnableObject
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private Transform camPos;
        [SerializeField] private Animator animator;
        [SerializeField] private string crouchParameterName = "Crouching";
        
        private Vector3 moveInput;
        private Vector2 rotationInput;
        private bool canMove;
        private bool crouching;

        private float yRot;
        
        private static Camera _cam;

        public static int PlayerID { get; set; }

        private void Awake()
        {
            if (_cam) return;
            
            _cam = Camera.main;
        }

        private void Update()
        {
            if (!canMove) return;
            
            if (!crouching)
                Move();
            
            Rotate();
        }

        private void Rotate()
        {
            if (rotationInput == Vector2.zero) return;

            transform.Rotate(Vector3.up * (Time.deltaTime * rotationInput.x));
            
            yRot -= Time.deltaTime * rotationInput.y;
            yRot = Mathf.Clamp(yRot, -80f, 80f);
            camPos.localRotation = Quaternion.Euler(yRot, 0f, 0f);
        }

        private void Move()
        {
            if (moveInput == Vector3.zero) return;

            Vector3 newPos = transform.position;

            newPos += transform.forward * (Time.deltaTime * speed * moveInput.z);
            newPos += transform.right * (Time.deltaTime * speed * moveInput.x);

            NetworkManager.Instance.SendData(new NetPosition(new Position(newPos.x, newPos.y, newPos.z, Data.Id)).Serialize());
        }

        private void OnDestroy()
        {
            UnPossess();
        }

        private void Possess()
        {
            if (!_cam) return;

            _cam.transform.parent = camPos;
            
            _cam.transform.localPosition = Vector3.zero;
            _cam.transform.localRotation = Quaternion.identity;

            InputListener.Move += OnMoveHandler;
            InputListener.Look += OnLookHandler;
            InputListener.Chat += OnChatHandler;
            InputListener.Crouch += OnCrouchHandler;
        }

        private void UnPossess()
        {
            if (!_cam) return;

            _cam.transform.parent = null;
            
            _cam.transform.position = Vector3.zero;
            _cam.transform.rotation = Quaternion.identity;

            InputListener.Move -= OnMoveHandler;
            InputListener.Look -= OnLookHandler;
            InputListener.Chat -= OnChatHandler;
            InputListener.Crouch -= OnCrouchHandler;
        }

        private void OnMoveHandler(Vector2 input)
        {
            moveInput.x = input.x;
            moveInput.z = input.y;
            moveInput.y = 0f;
        }

        private void OnLookHandler(Vector2 input)
        {
            rotationInput = input;
        }

        private void OnCrouchHandler()
        {
            crouching = !crouching;
            animator.SetBool(crouchParameterName, crouching);
        }
        
        private void OnChatHandler() => canMove = !canMove;

        public override void Spawn(SpawnableObjectData data)
        {
            base.Spawn(data);

            if (data.OwnerId == NetworkManager.Instance.Id)
                Possess();
        }
    }
}