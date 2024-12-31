using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerReady : NetworkBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI waitPlayerText;

    private void Start()
    {
        if (!IsHost)
        {
            readyButton.gameObject.SetActive(false);
        }
        else
        {
            waitPlayerText.gameObject.SetActive(false);
            readyButton.onClick.AddListener(() =>
            {
                GameStart();
            });
        }
    }

    private void GameStart()
    {
        if (IsHost)
        {
            
            NetworkManager.Singleton.SceneManager.LoadScene("gameScene", LoadSceneMode.Single);
        }
    }
}