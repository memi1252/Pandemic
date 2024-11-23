using System;
using Unity.Netcode;
using UnityEngine;
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
            NetworkManager.Singleton.StartHost();
        });
        join.onClick.AddListener(() =>
        {
            Debug.Log("Client");
            NetworkManager.Singleton.StartClient();
        });
    }
}
