using System;
using Game;
using Network.Messages;
using UnityEngine;

namespace Objects
{
    public class Player : SpawnableObject
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private Transform camPos;

        private Vector3 moveInput;
        private static Camera _cam;

        public static int PlayerID { get; set; }

        private void Awake()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            if (moveInput == Vector3.zero) return;

            transform.Translate(moveInput * (Time.deltaTime * speed));
            NetworkManager.Instance.SendData(
                new NetVector3(new Position(transform.position, ID, NetworkManager.Instance.ID)).Serialize());
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

            _cam.transform.position = camPos.position;
            _cam.transform.rotation = camPos.rotation;

            InputListener.Move += OnMoveHandler;
        }

        private void UnPossess()
        {
            if (!_cam) return;

            _cam.transform.position = Vector3.zero;
            _cam.transform.rotation = Quaternion.identity;

            InputListener.Move -= OnMoveHandler;
        }

        private void OnMoveHandler(Vector2 input)
        {
            moveInput.x = input.x;
            moveInput.z = input.y;
            moveInput.y = 0f;
        }
    }
}