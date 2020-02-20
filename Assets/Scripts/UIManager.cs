using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject StartMenu;
    public InputField UserNameField;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    public void ConnectToServer()
    {
        StartMenu.SetActive(false);
        UserNameField.interactable = false;
        Client.Instance.ConnectToServer();
    }
}
