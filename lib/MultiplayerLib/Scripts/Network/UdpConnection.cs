using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Multiplayer.Network.interfaces;
using Multiplayer.Utils;

namespace Multiplayer.Network
{
    public class UdpConnection
    {
        private struct DataReceived
        {
            public byte[] data;
            public IPEndPoint ipEndPoint;
        }

        private readonly UdpClient connection;
        private readonly IReceiveData receiver;
        private readonly Queue<DataReceived> dataReceivedQueue = new();

        private readonly object handler = new();
    
        public UdpConnection(int port, IReceiveData receiver = null)
        {
            connection = new UdpClient(port);

            this.receiver = receiver;

            connection.BeginReceive(OnReceive, null);
        }

        public UdpConnection(IPAddress ip, int port, IReceiveData receiver = null)
        {
            connection = new UdpClient();
            connection.Connect(ip, port);

            this.receiver = receiver;

            connection.BeginReceive(OnReceive, null);
        }

        public void Close()
        {
            connection.Close();
        }

        public void FlushReceiveData()
        {
            lock (handler)
            {
                while (dataReceivedQueue.Count > 0)
                {
                    DataReceived dataReceived = dataReceivedQueue.Dequeue();
                    
                    receiver?.OnReceiveData(dataReceived.data, dataReceived.ipEndPoint);
                }
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                DataReceived dataReceived = new DataReceived();
                dataReceived.data = connection.EndReceive(ar, ref dataReceived.ipEndPoint);

                lock (handler)
                {
                    dataReceivedQueue.Enqueue(dataReceived);
                }
            }
            catch(SocketException)
            {
                
            }

            connection.BeginReceive(OnReceive, null);
        }

        public void Send(byte[] data)
        {
            connection?.Send(data, data.Length);
        }

        public void Send(byte[] data, IPEndPoint ipEndpoint)
        {
            connection?.Send(data, data.Length, ipEndpoint);
        }
    }
}