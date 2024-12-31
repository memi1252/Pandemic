using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine.SceneManagement;

public class GameMenuUI : MonoBehaviour
{
    public static GameMenuUI Instance { get; private set; }

    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        Hide();
    }

    private void Awake()
    {
        Instance = this;
        settingsButton.onClick.AddListener(() =>
        {
            SettingUI.Instance.Show();
        });

        exitButton.onClick.AddListener(() =>
        {
            HostLeave();
        });
    }

    public void HostLeave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            // 현재 연결된 클라이언트 목록을 가져옴
            var clients = new List<ulong>(NetworkManager.Singleton.ConnectedClients.Keys);

            // 호스트가 나가기 전에 새로운 호스트를 선택
            if (clients.Count > 1) // 호스트 외에 다른 클라이언트가 있는 경우
            {
                for(int i =0 ; i < clients.Count; i++)
                {
                    NetworkManager.Singleton.DisconnectClient((ulong)i);
                }
                NetworkManager.Singleton.SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                NetworkManager.Singleton.Shutdown();
            }
            else
            {
                // 방에 남아 있는 플레이어가 없는 경우, 서버 종료
                SceneManager.LoadScene("MainScene");
                NetworkManager.Singleton.Shutdown();
            }
        }
        else // 클라이언트가 나가는 경우
        {
            SceneManager.LoadScene("MainScene");
            NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Hide();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        var player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        if (player.IsOwner)
        {
            player.playermove = false;
            var camera = NetworkManager.Singleton.LocalClient.PlayerObject.transform.GetChild(0)
                .GetComponent<PlayerCamera>();
            camera.playercamera = false;
            MuteAudioSources(true);
        }
    }

    public void Hide()
    {
        var player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        if (player.IsOwner)
        {
            player.playermove = true;
            var camera = NetworkManager.Singleton.LocalClient.PlayerObject.transform.GetChild(0)
                .GetComponent<PlayerCamera>();
            camera.playercamera = true;
            MuteAudioSources(false);
        }
        gameObject.SetActive(false);
    }

    private void MuteAudioSources(bool mute)
    {
        var audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            audioSource.mute = mute;
        }
    }
}
