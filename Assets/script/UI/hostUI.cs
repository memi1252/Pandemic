using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class hostUI : MonoBehaviour
{
    [SerializeField] private Button host;
    [SerializeField] private Button join;

    private void Awake()
    {
        host.onClick.AddListener(() =>
        {
            Debug.Log("Host");
            Cursor.lockState = CursorLockMode.Locked;
            NetworkManager.Singleton.StartHost();
        });
        join.onClick.AddListener(() =>
        {
            Debug.Log("Client");
            Cursor.lockState = CursorLockMode.Locked;
            NetworkManager.Singleton.StartClient();
        });
    }
}
