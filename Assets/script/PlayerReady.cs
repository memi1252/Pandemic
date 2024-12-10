using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerReady : NetworkBehaviour
{
    [SerializeField] private Button readyButton; // 준비 버튼
    [SerializeField] private Image[] playerImages; // 각 플레이어의 준비 상태를 나타내는 UI 이미지
    private NetworkVariable<int> totalPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // 총 플레이어 수
    private NetworkVariable<int> readyPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // 준비된 플레이어 수
    private Dictionary<ulong, NetworkVariable<bool>> clientReadyStatus = new Dictionary<ulong, NetworkVariable<bool>>(); // 각 클라이언트의 준비 상태를 서버에서 관리

    private void Start()
    {
        readyButton.onClick.AddListener(() =>
        {
            if (IsClient)
            {
                ToggleReadyStateServerRpc(); // 준비 상태를 서버에 알림
            }
        });
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            totalPlayers.Value = NetworkManager.Singleton.ConnectedClients.Count; // 서버에서 총 플레이어 수 초기화
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var readyStatus = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
                clientReadyStatus[clientId] = readyStatus;
                readyStatus.OnValueChanged += (oldValue, newValue) => UpdatePlayerUI(clientId, newValue);
            }

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
        else
        {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (clientReadyStatus.ContainsKey(clientId))
                {
                    clientReadyStatus[clientId].OnValueChanged += (oldValue, newValue) => UpdatePlayerUI(clientId, newValue);
                }
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            var readyStatus = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            clientReadyStatus[clientId] = readyStatus;
            readyStatus.OnValueChanged += (oldValue, newValue) => UpdatePlayerUI(clientId, newValue);
            totalPlayers.Value = NetworkManager.Singleton.ConnectedClients.Count;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (IsServer)
        {
            if (clientReadyStatus.ContainsKey(clientId))
            {
                clientReadyStatus.Remove(clientId);
                totalPlayers.Value = NetworkManager.Singleton.ConnectedClients.Count;
                readyPlayers.Value = CountReadyPlayers();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleReadyStateServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!clientReadyStatus.ContainsKey(clientId))
        {
            return;
        }

        // 준비 상태 토글
        bool isReady = !clientReadyStatus[clientId].Value;
        clientReadyStatus[clientId].Value = isReady;

        // 준비된 플레이어 수 업데이트
        readyPlayers.Value = CountReadyPlayers();

        // 모든 클라이언트에게 UI 업데이트 알림
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            UpdatePlayerUIClientRpc(clientId, isReady);
        }

        // 모든 플레이어가 준비되었는지 확인
        CheckAllClientsReady();
    }
    
    [ClientRpc]
    private void UpdatePlayerUIClientRpc(ulong clientId, bool ready)
    {
        UpdatePlayerUI(clientId, ready);
    }

    private int CountReadyPlayers()
    {
        int count = 0;
        foreach (var ready in clientReadyStatus.Values)
        {
            if (ready.Value) count++;
        }
        return count;
    }

    private void CheckAllClientsReady()
    {
        if (readyPlayers.Value >= totalPlayers.Value && totalPlayers.Value > 1) // 모든 플레이어가 준비된 경우
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    private void UpdatePlayerUI(ulong clientId, bool ready)
    {
        int playerIndex = (int)clientId;
        if (playerIndex < playerImages.Length)
        {
            playerImages[playerIndex].color = ready ? Color.green : Color.white;
        }
    }

    private void Update()
    {
        foreach (var clientId in clientReadyStatus.Keys)
        {
            UpdatePlayerUI(clientId, clientReadyStatus[clientId].Value);
        }
    }
}