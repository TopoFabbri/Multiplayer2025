﻿using System;
using Game;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.NetworkFactory;
using Multiplayer.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using Color = UnityEngine.Color;

namespace Objects
{
    public class Player : SpawnableObject
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float cameraSensitivity = 2f;
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private Transform camPos;
        [SerializeField] private Transform gunPoint;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private string crouchParameterName = "Crouching";
        [SerializeField] private int bulletPrefabIndex = 1;
        [SerializeField] private int health = 100;
        [SerializeField] private DamageCaster damageCaster;
        [SerializeField] private MeshRenderer meshRenderer;

        private float lastInputTimeStamp;

        public static event Action<int> Die; 
        
        private Vector3 moveInput;
        private Vector2 rotationInput;
        private bool canMove;
        private bool crouching;

        private static Camera _cam;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        private bool IsPossessed { get; set; }
        
        private bool IsGrounded => Physics.Raycast(rb.transform.position, Vector3.down, 1.2f);

        private void Awake()
        {
            Multiplayer.Network.Messages.Color color = ((ClientNetManager)NetworkManager.Instance).Color;

            meshRenderer.material.color = new Color(color.r, color.g, color.b, color.a);
            meshRenderer.material.SetColor(EmissionColor, new Color(color.r, color.g, color.b, color.a));
            
            if (_cam) return;

            _cam = Camera.main;
        }

        protected override void Update()
        {
            CheckAfk();
            
            if (canMove)
            {
                if (!crouching)
                    Move();

                Rotate();
            }

            base.Update();
        }

        private void CheckAfk()
        {
            if (Timer.Time - lastInputTimeStamp > ((ClientNetManager)NetworkManager.Instance).AfkTime && IsPossessed)
                ((ClientNetManager)NetworkManager.Instance).RequestDisconnect();
        }

        private void Rotate()
        {
            if (rotationInput == Vector2.zero) return;

            Data.Rot = new System.Numerics.Vector2(Data.Rot.X + rotationInput.x * cameraSensitivity * Time.deltaTime,
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

            IsPossessed = true;
            
            _cam.transform.parent = camPos;

            _cam.transform.localPosition = Vector3.zero;
            _cam.transform.localRotation = Quaternion.identity;

            InputListener.Move += OnMoveHandler;
            InputListener.Look += OnLookHandler;
            InputListener.Chat += OnChatHandler;
            InputListener.Crouch += OnCrouchHandler;
            InputListener.Jump += OnJumpHandler;
            InputListener.Shoot += OnShootHandler;
        }

        private void UnPossess()
        {
            if (!_cam) return;

            IsPossessed = false;
            
            _cam.transform.parent = null;

            _cam.transform.position = Vector3.zero;
            _cam.transform.rotation = Quaternion.identity;

            InputListener.Move -= OnMoveHandler;
            InputListener.Look -= OnLookHandler;
            InputListener.Chat -= OnChatHandler;
            InputListener.Crouch -= OnCrouchHandler;
            InputListener.Jump -= OnJumpHandler;
            InputListener.Shoot -= OnShootHandler;
        }

        private void OnMoveHandler(Vector2 input)
        {
            if (input != Vector2.zero)
                lastInputTimeStamp = Timer.Time;
            
            moveInput.x = input.x;
            moveInput.z = input.y;
            moveInput.y = 0f;
        }

        private void OnLookHandler(Vector2 input)
        {
            if (input != Vector2.zero)
                lastInputTimeStamp = Timer.Time;
            
            rotationInput = input;
        }

        public void Crouch()
        {
            crouching = !crouching;
            animator.SetBool(crouchParameterName, crouching);
        }

        public void Jump()
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private void OnCrouchHandler()
        {
            lastInputTimeStamp = Timer.Time;
            NetworkManager.Instance.SendData(new NetCrouch(Data.Id).Serialize());
        }

        private void OnJumpHandler()
        {
            lastInputTimeStamp = Timer.Time;
            
            if (IsGrounded && canMove)
                NetworkManager.Instance.SendData(new NetJump(Data.Id).Serialize());
        }

        private void OnShootHandler()
        {
            lastInputTimeStamp = Timer.Time;
            
            if (!canMove) return;
            
            SpawnableObjectData spawnableData = new()
            {
                OwnerId = NetworkManager.Instance.Id,
                PrefabId = bulletPrefabIndex,
                Pos = new Multiplayer.CustomMath.Vector3(gunPoint.position.x, gunPoint.position.y, gunPoint.position.z),
                Rot = new System.Numerics.Vector2(gunPoint.rotation.eulerAngles.y, gunPoint.rotation.eulerAngles.x)
            };

            ObjectManager.Instance.RequestSpawn(spawnableData);
        }

        private void OnChatHandler() => canMove = !canMove;

        public override void Spawn(SpawnableObjectData data)
        {
            base.Spawn(data);

            damageCaster.Id = data.Id;
            
            if (data.OwnerId != NetworkManager.Instance.Id) return;
            
            Possess();
            
            lastInputTimeStamp = Timer.Time;
        }

        public void Hit(int damage)
        {
            if (crouching)
                damage /= 2;
            
            health -= damage;
            
            if (health <= 0)
                Die?.Invoke(Data.Id);
        }
    }
}