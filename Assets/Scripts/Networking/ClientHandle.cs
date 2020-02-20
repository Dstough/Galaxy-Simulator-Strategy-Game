using System.Collections;
using System.Collections.Generic;
using System.Net;
using GalaxyServer.Networking;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        var message = packet.ReadString();
        var myId = packet.ReadInt();

        Debug.Log($"Message received from server: {message}");
        Client.Instance.Id = myId;
        ClientSend.WelcomeRecieved();
        Client.Instance.Udp.Connect(((IPEndPoint)Client.Instance.Tcp.Socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet packet)
    {
        var id = packet.ReadInt();
        var userName = packet.ReadString();
        var position = packet.ReadVector3();
        var rotation = packet.ReadQuaternion();

        GameManager.Instance.SpawnPlayer(id, userName, position, rotation);
    }
}
