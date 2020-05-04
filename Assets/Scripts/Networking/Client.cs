using System;
using System.Net;
using System.Net.Sockets;
using GalaxyServer.Networking;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client Instance;
    public static int DataBufferSize = 4096;

    public string ServerIp = "127.0.0.1";
    public int Port = 451;
    public int Id = 0;
    public TCP Tcp;
    public UDP Udp;

    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> PacketHandlers;
    private bool _isConnected = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    private void Start()
    {
        Tcp = new TCP();
        Udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        InitializeClientData();
        _isConnected = true;
        Tcp.Connect();
    }

    public class TCP
    {
        public TcpClient Socket;
        private Packet ReceiveData;
        private NetworkStream Stream;
        private byte[] ReceiveBuffer;

        public void Connect()
        {
            Socket = new TcpClient()
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };
            ReceiveBuffer = new byte[DataBufferSize];
            Socket.BeginConnect(Instance.ServerIp, Instance.Port, ConnectCallback, Socket);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            Socket.EndConnect(result);

            if (!Socket.Connected)
                return;

            Stream = Socket.GetStream();
            ReceiveData = new Packet();
            Stream.BeginRead(ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (Socket == null)
                    return;

                Stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to server via TCP: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var byteLength = Stream.EndRead(result);

                if (byteLength <= 0)
                {
                    Disconnect();
                    return;
                }

                var data = new byte[byteLength];

                Array.Copy(ReceiveBuffer, data, byteLength);

                ReceiveData.Reset(HandleData(data));

                Stream.BeginRead(ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error receiving TCP data: {ex}");
                Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            var packetLength = 0;

            ReceiveData.SetBytes(data);

            if (ReceiveData.UnreadLength() >= 4)
            {
                packetLength = ReceiveData.ReadInt();

                if (packetLength <= 0)
                    return true;
            }

            while (packetLength > 0 && packetLength <= ReceiveData.UnreadLength())
            {
                var packetBytes = ReceiveData.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (var packet = new Packet(packetBytes))
                    {
                        var packetId = packet.ReadInt();
                        PacketHandlers[packetId](packet);
                    }
                });

                packetLength = 0;

                if (ReceiveData.UnreadLength() >= 4)
                {
                    packetLength = ReceiveData.ReadInt();

                    if (packetLength <= 0)
                        return true;
                }
            }

            if (packetLength <= 1)
                return true;

            return false;
        }

        private void Disconnect()
        {
            Instance.Disconnect();

            Stream = null;
            ReceiveData = null;
            ReceiveBuffer = null;
            Socket = null;
        }
    }

    public class UDP
    {
        public UdpClient Socket;
        public IPEndPoint EndPoint;

        public UDP()
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(Instance.ServerIp), Instance.Port);
        }

        public void Connect(int localPort)
        {
            Socket = new UdpClient(localPort);
            Socket.Connect(EndPoint);
            Socket.BeginReceive(ReceiveCallback, null);
            
            using (var packet = new Packet()) 
                SendData(packet);
        }

        public void SendData(Packet packet)
        {
            try
            {
                packet.InsertInt(Instance.Id);

                if (Socket != null)
                    Socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error sending data to server via UDP: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var data = Socket.EndReceive(result, ref EndPoint);
                Socket.BeginReceive(ReceiveCallback, null);

                if (data.Length < 4)
                {
                    Disconnect();
                    return;
                }

                HandleData(data);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error reading UDP data from server: {ex}");
                Disconnect();
            }
        }

        private void HandleData(byte[] data)
        {
            using (var packet = new Packet(data))
            {
                var packetLength = packet.ReadInt();
                data = packet.ReadBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (var packet = new Packet(data))
                {
                    var packetId = packet.ReadInt();
                    PacketHandlers[packetId](packet);
                }
            });
        }

        private void Disconnect()
        {
            Instance.Disconnect();

            EndPoint = null;
            Socket = null;
        }
    }

    private void InitializeClientData()
    {
        PacketHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.Welcome, ClientHandle.Welcome },
            { (int)ServerPackets.SpawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.PlayerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.PlayerRotation, ClientHandle.PlayerRotation }
        };
    }
    
    private void Disconnect()
    {
        if (_isConnected)
        {
            _isConnected = false;
            Tcp.Socket.Close();
            Udp.Socket.Close();

            Debug.Log("Disconnected from server");
        }
    }
}
