using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [SerializeField] private GameObject lobbyListPanel; // Panel to display the lobby list
    [SerializeField] private GameObject lobbyItemPrefab; // Prefab for lobby items
    [SerializeField] private Button createRelayButton;

    private bool isInitialized = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private async void Start()
    {
        await InitializeUnityServices();

        // Ensure the user is signed in before querying lobbies
        if (AuthenticationService.Instance.IsSignedIn)
        {
            await QueryLobbies();
        }
        else
        {
            Debug.LogError("User is not signed in. Cannot query lobbies.");
        }
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            if (isInitialized) return; // Skip if already initialized

            await Unity.Services.Core.UnityServices.InitializeAsync();
            Debug.Log("Unity Services initialized successfully.");

            if (!Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn)
            {
                await Unity.Services.Authentication.AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Signed in anonymously.");
            }

            isInitialized = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during Unity Services initialization: {e.Message}");
        }
    }

    public async Task QueryLobbies()
    {
        try
        {
            // Ensure the user is signed in
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("User is not signed in. Attempting to sign in...");

                // Attempt to sign in
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Signed in anonymously.");
            }

            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0")
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(
                        asc: false,
                        field: QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);

            // Update the lobby list UI
            UpdateLobbyList(response.Results);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error querying lobbies: {e.Message}");
        }
    }

    private void UpdateLobbyList(List<Lobby> lobbies)
    {
        // Clear existing lobby items
        foreach (Transform child in lobbyListPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbies)
        {
            // Create lobby item
            GameObject lobbyItem = Instantiate(lobbyItemPrefab, lobbyListPanel.transform);

            // Initialize LobbyPanel
            LobbyPanel panel = lobbyItem.GetComponent<LobbyPanel>();
            if (panel != null)
            {
                panel.Initialize(lobby);
            }
            else
            {
                panel = LobbyMake.Instance.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetComponent<LobbyPanel>();
                return;
                
            }
        }
    }

    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId); // Store the joinCode
            Debug.Log(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );
            
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SpawnManager.PlayerObjects[(int)NetworkManager.Singleton.LocalClientId].GetComponent<Player>().playerNameText.text 
                = LobbyMake.Instance.playerNameInput.text;
            NetworkManager.Singleton.SceneManager.LoadScene("waitingRoomScene", LoadSceneMode.Single);
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );
            
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void LeaveLobby(string lobbyID)
    {
        try
        {
            // Ensure you sign-in before calling Authentication Instance
            // See IAuthenticationService interface
            string playerId = AuthenticationService.Instance.PlayerId;
            await LobbyService.Instance.RemovePlayerAsync(lobbyID, playerId);
            Debug.Log("Left the lobby.");
        }                                       
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DisconnectRelay(string lobbyId)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            LeaveLobby(lobbyId);
            SceneManager.LoadScene("MainScene");
            Debug.Log("Host has disconnected from the Relay server and left the lobby.");
        }
    }


}