using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.MessageInfo;
using Multiplayer.Utils;

namespace Multiplayer.Network
{
    public class ClientNetManager : NetworkManager
    {
        public IPAddress IpAddress { get; private set; }

        private readonly Dictionary<int, Color> colorsByClients = new();
        private readonly Dictionary<int, string> namesById = new();
        public Dictionary<int, float> PingsByClientId { get; private set; } = new();
        public int PlayerId { private get; set; }
        public bool IsConnectedToServer { get; private set; }
        
        private float LastPingTime { get; set; }
        private int MmPort { get; set; }
        private bool ConnectToServer { get; set; }
        private IPEndPoint MmIp { get; set; }
        public Color Color { get; set; } = new();
        public float AfkTime => 15f;

        private int level;

        public string GetName(int id)
        {
            return namesById.TryGetValue(id, out string nameTmp) ? nameTmp : "Client " + id;
        }
        
        public event Action Disconnected;

        protected override void Start()
        {
            base.Start();

            Random random = new(Timer.DateTime.Millisecond);
            level = random.Next(0, 10);
        }

        public override void Init(int port, IPAddress ip = null, string name = "Player")
        {
            Port = port;
            IpAddress = ip;

            Id = -1;

            connection = new UdpConnection(ip, port, this);

            MessageHandler.TryAddHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryAddHandler(MessageType.Ping, HandlePing);
            MessageHandler.TryAddHandler(MessageType.ServerInfo, HandleServerInfo);
            MessageHandler.TryAddHandler(MessageType.Disconnect, HandleDisconnect);

            MessageHandler.TryAddOnAcknowledgeHandler(MessageType.Disconnect, HandleAcknowledgedDisconnect);

            Dictionary<int, Color> newColor = new() { { Id, Color } };

            Dictionary<int, string> names = new();
            
            SendTo(new NetHandShake(new HandShake(0, newColor, new Dictionary<int, string>(), false, level, name), false).Serialize());

            LastPingTime = Timer.Time;

            base.Init(port, ip, name);
        }

        public override void Update()
        {
            base.Update();

            if (LastPingTime <= 0) return;

            if (Timer.Time - LastPingTime > TimeOut)
                Disconnect();
        }
        
        private void HandleHandshake(byte[] data, IPEndPoint ip)
        {
            HandShake hs = new NetHandShake(data).Deserialized();

            CheckSum.RandomSeed = hs.randomSeed;
            CheckSum.CreateOperationsArrays(hs.randomSeed);
            Crypt.GenerateOperations(hs.randomSeed);

            foreach (KeyValuePair<int, Color> client in hs.clientColorsById)
                colorsByClients.TryAdd(client.Key, client.Value);

            foreach (KeyValuePair<int, string> nameAndId in hs.clientNames)
                namesById.TryAdd(nameAndId.Key, nameAndId.Value);

            LastPingTime = Timer.Time;

            if (Id > 0) return;

            Id = colorsByClients.Last().Key;

            if (hs.fromServer)
            {
                onConnectionEstablished?.Invoke();
            }
            else
            {
                MmPort = Port;
                MmIp = ip;
            }
        }

        private void HandlePing(byte[] data, IPEndPoint ip)
        {
            PingWrapper ping = new NetPing(data).Deserialized();
            PingsByClientId = ping.PingsByClientId;

            Ping = Timer.Time - LastPingTime;
            LastPingTime = Timer.Time;

            if (!PingsByClientId.TryAdd(Id, Ping))
                PingsByClientId[Id] = Ping;
        }

        private void HandleServerInfo(byte[] data, IPEndPoint ip)
        {
            Port = new NetServerInfo(data).Deserialized().port;
            ConnectToServer = true;

            RequestDisconnect();
        }

        private void HandleDisconnect(byte[] data, IPEndPoint ip)
        {
            RequestDisconnect();
        }

        private void HandleAcknowledgedDisconnect(byte[] data, IPEndPoint ip)
        {
            Disconnect();
        }

        public override void SendData(byte[] data)
        {
            SendTo(data);
        }

        public override void SendTo(byte[] data, IPEndPoint ip = null)
        {
            base.SendTo(data, ip);

            if (Crypt.IsCrypted(data))
                data = Crypt.Encrypt(data);

            connection.Send(data);
        }

        public void RequestDisconnect()
        {
            SendTo(new NetDisconnect(PlayerId).Serialize());
        }

        private void Disconnect()
        {
            Disconnected?.Invoke();
            
            colorsByClients.Clear();
            PingsByClientId.Clear();
            namesById.Clear();

            connection?.Close();

            MessageHandler.TryRemoveHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryRemoveHandler(MessageType.Ping, HandlePing);
            MessageHandler.TryRemoveHandler(MessageType.ServerInfo, HandleServerInfo);

            MessageHandler.TryRemoveOnAcknowledgeHandler(MessageType.Disconnect, HandleAcknowledgedDisconnect);
            
            if (IsConnectedToServer)
            {
                IsConnectedToServer = false;

                Init(MmPort, MmIp.Address, Name);
            }
            else if (ConnectToServer)
            {
                Init(Port, IpAddress, Name);
                IsConnectedToServer = true;
                ConnectToServer = false;
            }
        }

        protected override void OnShouldAcknowledge(MessageMetadata metadata, IPEndPoint ip)
        {
            if (IsConnectedToServer && Equals(ip, MmIp))
                return;

            base.OnShouldAcknowledge(metadata, ip);
        }
    }
}