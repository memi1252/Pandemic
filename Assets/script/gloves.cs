// gloves.cs
using UnityEngine;
using Unity.Netcode;

public class gloves : NetworkBehaviour
{
    public static gloves Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
    }

    public void Interaction()
    {
        var player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        if (player != null && !player.isgloves.Value)
        {
            player.SetGlove(true);
            DestroyServerRpc();
        }
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyServerRpc()
    {
        DestroyObject();
        DestroyClientRpc();
    }

    [ClientRpc]
    private void DestroyClientRpc()
    {
        DestroyObject();
    }
}