using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI passworld;

    public string lobbyId;
    private Lobby _lobby;

    private void Awake()
    {
        
        transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("Join button clicked");
            LobbyMake.Instance.waitImage.gameObject.SetActive(true);
            LobbyMake.Instance.lobbyJoin(lobbyId);
        });
    }

    public void Initialize(Lobby lobby)
    {
        if (lobby == null)
        {
            Debug.LogError("Invalid lobby data. Cannot initialize lobby panel.");
            return;
        }

        _lobby = lobby;
        lobbyNameText.text = lobby.Name;
        playerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
        lobbyId = lobby.Id;
        passworld.text = lobby.IsPrivate ? "비공개" : "공개";
    }
}