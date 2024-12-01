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
        GravitationCore.SetActive(true);
        HideServerRpc();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HideServerRpc()
    {
        Hide();
        HideClientRpc();
    }

    [ClientRpc]
    private void HideClientRpc()
    {
        Hide();
    }
}