using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts.Networking.Server
{
    class Server : MonoBehaviour
    {
        public int MaxPlayers;
        public int Port;

        private TcpListener _tcpListener;
        private UdpClient _udpListener;

        private List<User> _users;

        public void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            _users = new List<User>();

            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(NewTcpConnection, null);

            _udpListener = new UdpClient(Port);
            _udpListener.BeginReceive(NewUdpMessage, null);

            Debug.Log($"Server Started on Port {Port}");
        }

        private void NewTcpConnection(IAsyncResult result)
        {
            var client = _tcpListener.EndAcceptTcpClient(result);

            _tcpListener.BeginAcceptTcpClient(NewTcpConnection, null);

            Debug.Log($"Incoming connection from {client.Client.RemoteEndPoint}...");

            if (_users.Count < MaxPlayers)
                _users.Add(new User(_users.Count + 1));
        }

        private void NewUdpMessage(IAsyncResult result)
        {

        }
    }
}
