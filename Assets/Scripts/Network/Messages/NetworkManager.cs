﻿using System;
using System.Net;
using Network.interfaces;
using UnityEngine;
using Utils;

namespace Network.Messages
{
    public struct Client
    {
        public float timeStamp;
        public int id;
        public readonly IPEndPoint ipEndPoint;
        public float lastPingTime;

        public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
        {
            this.timeStamp = timeStamp;
            this.id = id;
            this.ipEndPoint = ipEndPoint;
            lastPingTime = Time.time;
        }
    }

    public abstract class NetworkManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
    {
        protected int ID { get; set; }
        
        protected int Port { get; set; }
        public float Ping { get; protected set; }
        
        public Action onConnectionEstablished;
        public Action<byte[]> OnReceiveDataAction;

        protected const float TimeOut = 5;

        protected UdpConnection connection;
        
        protected override void Initialize()
        {
            MessageHandler.TryAddHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryAddHandler(MessageType.Console, HandleConsole);
            MessageHandler.TryAddHandler(MessageType.Position, HandlePosition);
            MessageHandler.TryAddHandler(MessageType.SpawnRequest, HandleSpawnable);
            MessageHandler.TryAddHandler(MessageType.Ping, HandlePing);
            MessageHandler.TryAddHandler(MessageType.Disconnect, HandleDisconnect);
        }

        protected abstract void HandleHandshake(byte[] data, IPEndPoint ip);

        protected virtual void HandleConsole(byte[] data, IPEndPoint ip)
        {
        }

        protected abstract void HandlePosition(byte[] data, IPEndPoint ip);

        protected virtual void HandleSpawnable(byte[] data, IPEndPoint ip)
        {
        }
        
        protected abstract void HandlePing(byte[] data, IPEndPoint ip);

        protected virtual void HandleDisconnect(byte[] data, IPEndPoint ip)
        {
        }

        public virtual void OnReceiveData(byte[] data, IPEndPoint ip)
        {
            OnReceiveDataAction?.Invoke(data);
            
            MessageHandler.Receive(data, ip);
        }

        protected virtual void Update()
        {
            // Flush the data in main thread
            connection?.FlushReceiveData();
        }

        public abstract void SendData(byte[] data);

        public virtual void Init(int port, IPAddress ip = null)
        {
            onConnectionEstablished?.Invoke();
        }
    }
}