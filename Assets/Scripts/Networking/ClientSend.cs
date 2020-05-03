using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyServer.Networking;

public class ClientSend : MonoBehaviour
{
    private static void SendTcpData(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.Tcp.SendData(packet);
    }

    private static void SendUdpData(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.Udp.SendData(packet);
    }

    #region Packets

    public static void WelcomeRecieved()
    {
        using (var packet = new Packet((int)ClientPackets.WelcomeReceived))
        {
            packet.Write(Client.Instance.Id);
            packet.Write(UIManager.Instance.UserNameField.text);
            SendTcpData(packet);
        }
    }

    public static void PlayerMovement(bool[] inputs)
    {
        using (var packet = new Packet((int)ClientPackets.PlayerMovement))
        {
            packet.Write(inputs.Length);
            
            foreach (var item in inputs) 
                packet.Write(item);

            packet.Write(GameManager.Players[Client.Instance.Id].transform.rotation);
            
            SendUdpData(packet);
        }
    }

    #endregion
}
