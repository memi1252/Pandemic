using System;
using Unity.Netcode;
using UnityEngine;

public class Shoes : NetworkBehaviour
{
    public static Shoes Instance { get; private set; }

    public GameObject GravitationCore;

    private void Awake()
    {
        Instance = this;
    }

    public void Interaction()
    {
        var player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        if (player != null && !player.isshoes.Value)
        {
            player.SetShoes(true);
            GravitationCore.SetActive(true);
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