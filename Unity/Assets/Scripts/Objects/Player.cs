using System.Net;
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
        [SerializeField] private float cameraSensitivity = 2f;
        [SerializeField] private Transform camPos;
        [SerializeField] private Animator animator;
        [SerializeField] private string crouchParameterName = "Crouching";
        
        private Vector3 moveInput;
        private Vector2 rotationInput;
        private bool canMove;
        private bool crouching;
        
        private static Camera _cam;

        public static int PlayerID { get; set; }
        
        private void Awake()
        {
            if (_cam) return;
            
            _cam = Camera.main;
        }

        protected override void Update()
        {
            if (canMove)
            {
                if (!crouching)
                    Move();

                Rotate();
            }
            
            base.Update();
        }

        private void Rotate()
        {
            if (rotationInput == Vector2.zero) return;
            
            Data.Rot = new System.Numerics.Vector2(
                Data.Rot.X + rotationInput.x * cameraSensitivity * Time.deltaTime,
                Data.Rot.Y + rotationInput.y * cameraSensitivity * Time.deltaTime);

            if (rotationInput != Vector2.zero)
                NetworkManager.Instance.SendData(new NetRotation(new Rotation(Data.Rot, Data.Id)).Serialize());
        }

        private void Move()
        {
            if (moveInput == Vector3.zero) return;

            Vector3 newPos = transform.position;

            newPos += transform.forward * (Time.deltaTime * speed * moveInput.z);
            newPos += transform.right * (Time.deltaTime * speed * moveInput.x);

            NetworkManager.Instance.SendData(new NetPosition(new Position(newPos.x, newPos.y, newPos.z, Data.Id)).Serialize());
        }

        protected override void ApplyPosition()
        {
            transform.position = new Vector3(Data.Pos.x, Data.Pos.y, Data.Pos.z);
        }

        protected override void ApplyRotation()
        {
            transform.localRotation = Quaternion.AngleAxis(Data.Rot.X, Vector3.up);
            camPos.localRotation = Quaternion.AngleAxis(Data.Rot.Y, Vector3.right);
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

        public void Crouch()
        {
            crouching = !crouching;
            animator.SetBool(crouchParameterName, crouching);
        }
        
        private void OnCrouchHandler()
        {
            NetworkManager.Instance.SendData(new NetCrouch(Data.Id).Serialize());
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