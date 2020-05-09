using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public bool isServer;
    public IPAddress serverIp;
    public TcpListener listener;

    internal List<ConnectedClient> clientList = new List<ConnectedClient>();

    public Text MessageToDisplay;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        if (serverIp == null)
        {
            instance.isServer = true;
            instance.listener = new TcpListener(IPAddress.Any, NetworkConfig.PORT);
            instance.listener.Start();
            instance.listener.BeginAcceptTcpClient(OnServerConnect, null);
        }
        else
        {
            instance.isServer = false;
            var client = new TcpClient();
            var connectedClient = new ConnectedClient(client);
            instance.clientList.Add(connectedClient);
            client.BeginConnect(serverIp, NetworkConfig.PORT, (arg) => connectedClient.EndConnect(arg), null);
        }
    }

    void OnServerConnect(IAsyncResult arg)
    {
        var tcpClient = listener.EndAcceptTcpClient(arg);
        instance.listener.BeginAcceptTcpClient(OnServerConnect, null);
        instance.clientList.Add(new ConnectedClient(tcpClient));
    }

    public void OnDisconnect(ConnectedClient client)
    {
        instance.clientList.Remove(client);
    }

    void Send(string message)
    {
        foreach (var client in instance.clientList)
        {
            client.Send(message);
        }

        if (isServer)
        {
            instance.MessageToDisplay.text += message + Environment.NewLine;
        }
    }

    void Update()
    {

    }

    private void OnApplicationQuit()
    {
        instance.listener?.Stop();

        foreach (var client in instance.clientList)
        {
            client.Close();
        }
    }
}
