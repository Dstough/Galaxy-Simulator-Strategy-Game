using System.Net;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject lblMyIpAddress;
    public GameObject txtUsername;
    public GameObject txtIpAddress;

    void Start()
    {
        var httpClient = new HttpClient();
        var task = httpClient.GetStringAsync("https://api.ipify.org");

        task.Wait();

        lblMyIpAddress.GetComponent<Text>().text = task.Result;
    }

    void OnHostClick()
    {
        NetworkConfig.UserName = txtUsername.GetComponent<Text>().text;
        NetworkConfig.IpAddress = lblMyIpAddress.GetComponent<Text>().text;
        NetworkConfig.IsServer = true;

        //TODO: move to the chat scene.
    }

    void OnJoinClick()
    {
        NetworkConfig.UserName = txtUsername.GetComponent<Text>().text;
        NetworkConfig.IpAddress = lblMyIpAddress.GetComponent<Text>().text;
        NetworkConfig.IsServer = false;

        //TODO: move to the chat scene.
    }
}
