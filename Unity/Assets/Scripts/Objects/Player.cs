using Game;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using UnityEngine;

namespace Objects
{
    public class Player : SpawnableObject
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private Transform camPos;

        private Vector3 moveInput;
        private float rotationInput;
        private bool canMove;
        
        private static Camera _cam;

        public bool IsPossessed => PlayerID == ID;
        
        public static int PlayerID { get; set; }

        private void Awake()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            if (!canMove) return;
            
            Move();
            Rotate();
        }

        private void Rotate()
        {
            if (rotationInput == 0f) return;

            transform.Rotate(Vector3.up * (Time.deltaTime * rotationInput));
        }

        private void Move()
        {
            if (moveInput == Vector3.zero) return;

            Vector3 newPos = transform.position;

            newPos += transform.forward * (Time.deltaTime * speed * moveInput.z);
            newPos += transform.right * (Time.deltaTime * speed * moveInput.x);

            NetworkManager.Instance.SendData(new NetPosition(new Position(newPos.x, newPos.y, newPos.z, ID)).Serialize());
        }

        private void OnDestroy()
        {
            UnPossess();
        }

        public override SpawnableObject Spawn(ObjectManager objectManager, int id)
        {
            SpawnableObject spawnedObject = base.Spawn(objectManager, id);

            if (PlayerID == id)
                (spawnedObject as Player)?.Possess();

            spawnedObject.transform.position += Vector3.up;

            return spawnedObject;
        }

        private void Possess()
        {
            if (!_cam) return;

            _cam.transform.parent = transform;
            
            _cam.transform.position = camPos.position;
            _cam.transform.rotation = camPos.rotation;

            InputListener.Move += OnMoveHandler;
            InputListener.Look += OnLookHandler;
            InputListener.Chat += OnChatHandler;
        }

        private void UnPossess()
        {
            if (!_cam || !IsPossessed) return;

            _cam.transform.parent = null;
            
            _cam.transform.position = Vector3.zero;
            _cam.transform.rotation = Quaternion.identity;

            InputListener.Move -= OnMoveHandler;
            InputListener.Look -= OnLookHandler;
            InputListener.Chat -= OnChatHandler;
        }

        private void OnMoveHandler(Vector2 input)
        {
            moveInput.x = input.x;
            moveInput.z = input.y;
            moveInput.y = 0f;
        }

        private void OnLookHandler(Vector2 input)
        {
            rotationInput = input.x;
        }
        
        private void OnChatHandler() => canMove = !canMove;
    }
}