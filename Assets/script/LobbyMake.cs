using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMake : MonoBehaviour
{
    public static LobbyMake Instance { get; private set; }

    [SerializeField] private Button make;
    [SerializeField] private Button join;
    [SerializeField] private Button publicLobbyButton;
    [SerializeField] private Button creatLobbyCloseButton;
    [SerializeField] private Button ResetLobbyButton;
    [SerializeField] private TMP_InputField lobbyNameInput;
    [SerializeField] private Image creatLobby;
    [SerializeField] public TMP_InputField playerNameInput;
    [SerializeField] public Image waitImage;

    private bool isSigningIn = false;
    private bool isInitialized = false;

    private async void Awake()
    {
        Instance = this;
        waitImage.gameObject.SetActive(false);

        make.onClick.AddListener(() =>
        {
            creatLobby.gameObject.SetActive(true);
        });
        join.onClick.AddListener(() =>
        {
            QuickJoin();
        });
        publicLobbyButton.onClick.AddListener(() =>
        {
            CreatePublicLobby();
            waitImage.gameObject.SetActive(true);
        });
        creatLobbyCloseButton.onClick.AddListener(() =>
        {
            creatLobby.gameObject.SetActive(false);
        });
        ResetLobbyButton.onClick.AddListener(async () =>
        {
            await LobbyManager.Instance.QueryLobbies();
        });
    }

    private async void CreatePublicLobby()
    {
        try
        {
            string lobbyName = lobbyNameInput.text;
            int maxPlayers = 4;

            // Relay 코드 생성 (미리 생성하여 로비 데이터에 포함)
            string joinCode = await LobbyManager.Instance.CreateRelay();

            // 로비 생성 시 초기 데이터로 설정
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            };

            // 로비 생성
            await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            Debug.Log($"Public lobby '{lobbyName}' created successfully with join code: {joinCode}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create public lobby: {e}");
        }
    }


    public async void lobbyJoin(string lobbyId)
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
        
            // Retrieve the joinCode from the lobby data
            if (lobby.Data.TryGetValue("joinCode", out var joinCodeData))
            {
                string joinCode = joinCodeData.Value;
                LobbyManager.Instance.JoinRelay(joinCode); // Pass the joinCode to join the relay
            }
            else
            {
                Debug.LogError("JoinCode not found in lobby data.");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async Task QuickJoin()
    {
        try
        {
            QueryLobbiesOptions queryOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0")
                }
            };

            var queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
            var availableLobby = queryResponse.Results.FirstOrDefault(lobby =>
            {
                int maxPlayers = int.Parse(lobby.Data["MaxPlayers"].Value);
                int currentPlayers = lobby.Players.Count;
                return maxPlayers > currentPlayers;
            });

            if (availableLobby != null)
            {
                string joinCode = availableLobby.Data["joinCode"].Value;
                LobbyManager.Instance.JoinRelay(joinCode);
            }
            else
            {
                Debug.Log("No available lobbies found.");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Error during QuickJoin: {e.Message}");
        }
    }


}