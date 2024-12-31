using System.Collections;
using System.Linq;
using TextInspectSystem;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class MemoryStage : NetworkBehaviour
{
    public static MemoryStage Instance { get; private set; }

    [SerializeField] private Material[] colors = new Material[9];
    [SerializeField] private Material defaultColor;
    [SerializeField] private GameObject view;
    [SerializeField] private Material[] memoryColors = new Material[13];
    [SerializeField] private Material[] checkColors = new Material[13];
    [SerializeField] private int colorNum = 0;
    [SerializeField] private int maxColorNum = 0;
    [SerializeField] private int showColorNum = 0;
    [SerializeField] private int conut = 0;
    [SerializeField] private int clickCount = 0;
    public NetworkVariable<bool> start = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        Instance = this;
    }

    public void MemoryStart()
    {
        if (IsServer)
        {
            StartMemoryGame();
        }
        else
        {
            StartMemoryGameServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartMemoryGameServerRpc()
    {
        StartMemoryGame();
    }

    private void StartMemoryGame()
    {
        start.Value = true;
        conut = 0;
        memoryColors[colorNum] = colors[Random.Range(0, colors.Length)];
        view.GetComponent<Renderer>().material = memoryColors[colorNum];
        StartMemoryGameClientRpc(memoryColors[colorNum].name);
    }

    [ClientRpc]
    private void StartMemoryGameClientRpc(string colorName)
    {
        conut = 0;
        memoryColors[colorNum] = colors.FirstOrDefault(c => c.name == colorName);
        view.GetComponent<Renderer>().material = memoryColors[colorNum];
    }

    public void MemoryReset()
    {
        if (IsServer)
        {
            ResetMemoryGame();
        }
        else
        {
            ResetMemoryGameServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetMemoryGameServerRpc()
    {
        ResetMemoryGame();
    }

    private void ResetMemoryGame()
    {
        start.Value = false;
        memoryColors = new Material[13];
        view.GetComponent<Renderer>().material = defaultColor;
        checkColors = new Material[13];
        colorNum = 0;
        maxColorNum = 0;
        showColorNum = 0;
        conut = 0;
        clickCount = 0;
        ResetMemoryGameClientRpc();
    }

    [ClientRpc]
    private void ResetMemoryGameClientRpc()
    {
        memoryColors = new Material[13];
        view.GetComponent<Renderer>().material = defaultColor;
        checkColors = new Material[13];
        colorNum = 0;
        maxColorNum = 0;
        showColorNum = 0;
        conut = 0;
        clickCount = 0;
    }

    public void CheckColor(TextInspectItem item)
    {
        if (IsServer)
        {
            CheckColorServer(item);
        }
        else
        {
            CheckColorServerRpc(item.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckColorServerRpc(ulong itemNetworkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkObjectId, out var item))
        {
            CheckColorServer(item.GetComponent<TextInspectItem>());
        }
    }

    private void CheckColorServer(TextInspectItem item)
    {
        var currentColor = memoryColors[colorNum];
        if (item.memorys.red && currentColor.name == "red") UpCheckColor(item);
        else if (item.memorys.ornage && currentColor.name == "orange") UpCheckColor(item);
        else if (item.memorys.yellow && currentColor.name == "yellow") UpCheckColor(item);
        else if (item.memorys.green && currentColor.name == "green") UpCheckColor(item);
        else if (item.memorys.blue && currentColor.name == "blue") UpCheckColor(item);
        else if (item.memorys.darkblue && currentColor.name == "darkblue") UpCheckColor(item);
        else if (item.memorys.purple && currentColor.name == "puple") UpCheckColor(item);
        else if (item.memorys.brown && currentColor.name == "brown") UpCheckColor(item);
        else if (item.memorys.pick && currentColor.name == "lightpink") UpCheckColor(item);
        else GameOver();
    }

    private void GameOver()
    {
        MemoryReset();
    }

    private void UpCheckColor(TextInspectItem item)
    {
        checkColors[colorNum] = item.transform.GetChild(1).gameObject.GetComponent<Renderer>().material;
        colorNum++;
        clickCount++;
        if (conut + 1 == clickCount)
        {
            memoryColors[colorNum] = colors[Random.Range(0, colors.Length)];
            StartCoroutine(ShowColorSequence());
        }
    }

    private IEnumerator ShowColorSequence()
    {
        showColorNum = 0;
        while (memoryColors[showColorNum] != null)
        {
            ShowColorClientRpc(memoryColors[showColorNum].name);
            yield return new WaitForSeconds(1);
            showColorNum++;
        }
        ResetGameStateClientRpc();
    }

    [ClientRpc]
    private void ShowColorClientRpc(string colorName)
    {
        Material colorToShow = colors.FirstOrDefault(c => c.name == colorName);
        if (colorToShow != null)
        {
            view.GetComponent<Renderer>().material = colorToShow;
        }
    }

    [ClientRpc]
    private void ResetGameStateClientRpc()
    {
        colorNum = 0;
        showColorNum = 0;
        conut++;
        clickCount = 0;
        checkColors = new Material[13];
    }
}