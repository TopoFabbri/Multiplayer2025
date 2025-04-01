using System;
using System.Collections.Generic;
using System.Net;
using Network;
using UnityEngine;

public struct Client
{
    public float timeStamp;
    public int id;
    public IPEndPoint ipEndPoint;

    public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
    {
        this.timeStamp = timeStamp;
        this.id = id;
        this.ipEndPoint = ipEndPoint;
    }
}

public class NetworkManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
{
    public IPAddress IPAddress
    {
        get; private set;
    }

    public int Port
    {
        get; private set;
    }

    public bool IsServer
    {
        get; private set;
    }

    public int timeOut = 30;

    public Action<byte[], IPEndPoint> OnReceiveEvent;

    private UdpConnection connection;

    private readonly Dictionary<int, Client> clients = new();
    private readonly Dictionary<IPEndPoint, int> ipToId = new();

    private int clientId; // This id should be generated during first handshake

    public void StartServer(int port)
    {
        IsServer = true;
        Port = port;
        connection = new UdpConnection(port, this);
    }

    public void StartClient(IPAddress ip, int port)
    {
        IsServer = false;

        Port = port;
        IPAddress = ip;

        connection = new UdpConnection(ip, port, this);

        AddClient(new IPEndPoint(ip, port));
    }

    private void AddClient(IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip)) return;
        
        Debug.Log("Adding client: " + ip.Address);

        int id = clientId;
        ipToId[ip] = clientId;

        clients.Add(clientId, new Client(ip, id, Time.realtimeSinceStartup));

        clientId++;
    }

    void RemoveClient(IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            Debug.Log("Removing client: " + ip.Address);
            clients.Remove(ipToId[ip]);
        }
    }

    public void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        AddClient(ip);

        if (OnReceiveEvent != null)
            OnReceiveEvent.Invoke(data, ip);
    }

    public void SendToServer(byte[] data)
    {
        connection.Send(data);
    }

    public void Broadcast(byte[] data)
    {
        foreach (KeyValuePair<int, Client> keyValuePair in clients)
            connection.Send(data, keyValuePair.Value.ipEndPoint);
    }

    void Update()
    {
        // Flush the data in main thread
        if (connection != null)
            connection.FlushReceiveData();
    }
}

public class ServerNetManager : NetworkManager
{
    public void Start()
    {
    }
    
    private void Broadcast(byte[] data)
    {
    }
}

public class ClientNetManager : NetworkManager
{
    public int ID { get; private set; }
    
    public void Start()
    {
        
    }
}