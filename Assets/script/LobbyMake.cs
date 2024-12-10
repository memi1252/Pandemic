
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMake : MonoBehaviour
{
    [SerializeField] private Button make;
    [SerializeField] private Button join;
    [SerializeField] private Button privateLobbyButton;
    [SerializeField] private Button publicLobbyButton;
    [SerializeField] private Image backGround;

    private async void Awake()
    {
        //DontDestroyOnLoad(NetworkManager.Singleton.gameObject);
        
        await InitializeUnityServices();

        make.onClick.AddListener(() =>
        {
            backGround.gameObject.SetActive(true);
        });
        join.onClick.AddListener(() =>
        {
            ds();
        });
        privateLobbyButton.onClick.AddListener(() =>
        {
            privateLobby();
        });
        publicLobbyButton.onClick.AddListener(() =>
        {
            publicLobby();
        });
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Unity Services initialized and user signed in.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize Unity Services: {e}");
        }
    }
    
    
    private async Task publicLobby()
    {
        try
        {
            string lobbyName = "new lobby";
            int maxPlayers = 4;
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;
            
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("waitingRoomScene", LoadSceneMode.Single);
            Debug.Log("Public lobby created");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create public lobby: {e}");
        }
    }

    private async Task privateLobby()
    {
        try
        {
            string lobbyName = "new lobby";
            int maxPlayers = 4;
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = true;

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            NetworkManager.Singleton.SceneManager.LoadScene("waitingRoomScene", LoadSceneMode.Single);
            Debug.Log("Private lobby created");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create private lobby: {e}");
        }
    }

    private async Task ds()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
            options.Filter = new List<QueryFilter>()
            {
                new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GT, value: "0")
            };

            var lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            NetworkManager.Singleton.StartClient();
            NetworkManager.Singleton.SceneManager.LoadScene("waitingRoomScene", LoadSceneMode.Single);
            Debug.Log("Joined lobby successfully");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to join lobby: {e}");
        }
    }
}