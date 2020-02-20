using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>();
    public GameObject LocalPlayerPrefab;
    public GameObject RemotePlayerPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    public void SpawnPlayer(int id, string userName, Vector3 position, Quaternion rotation)
    {
        GameObject player;
        if (id == Client.Instance.Id)
            player = Instantiate(LocalPlayerPrefab, position, rotation);
        else
            player = Instantiate(RemotePlayerPrefab, position, rotation);

        player.GetComponent<PlayerManager>().Id = id;
        player.GetComponent<PlayerManager>().UserName = userName;
        Players.Add(id, player.GetComponent<PlayerManager>());
    }
}
