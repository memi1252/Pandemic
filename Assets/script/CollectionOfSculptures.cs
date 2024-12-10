using System;
using Unity.Netcode;
using UnityEngine;

public class CollectionOfSculptures : NetworkBehaviour
{
    public static CollectionOfSculptures Instance { get; private set; }
    
    [SerializeField] public bool stone1;
    [SerializeField] public bool stone2;
    [SerializeField] public bool stone3;
    [SerializeField] public bool stone4;
    private bool openDoor;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!openDoor)
        {
            if(stone1 && stone2 && stone3 && stone4)
            {
                OpneDoor();
                openDoor = true;
            }
        }
    }

    private void OpneDoor()
    {
        Debug.Log("오픈 도어");
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void UpdateStone1ServerRpc(bool value, int index)
    {

        if (index == 1)
        {
            stone1 = value;
        }else if (index == 2)
        {
            stone2 = value;
        }
        else if (index == 3)
        {
            stone3 = value;
        }
        else if (index == 4)
        {
            stone4 = value;
        }
        
        UpdateStone1ClientRpc(value, index);
    }

    [ClientRpc]
    private void UpdateStone1ClientRpc(bool value,int index)
    {
        if (index == 1)
        {
            stone1 = value;
        }else if (index == 2)
        {
            stone2 = value;
        }
        else if (index == 3)
        {
            stone3 = value;
        }
        else if (index == 4)
        {
            stone4 = value;
        }
    }
}
