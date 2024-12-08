using System.Collections;
using TextInspectSystem;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class TextInspectInteractor : NetworkBehaviour
{
    public static TextInspectInteractor Instance { get; private set; }

    [Header("Raycast Features")] [SerializeField]
    private float rayLength = 5;

    private Camera _camera;

    private TextInspectItem textItem;
    public GameObject tagetName;

    [Header("Input Key")] [SerializeField] private KeyCode interactKey;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!TryGetComponent<Camera>(out _camera))
        {
            Debug.LogError("Camera component not found on the GameObject.");
        }
    }

    private void Update()
    {
        // Raycast를 통해 상호작용 가능한 오브젝트를 감지
        if (Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), _camera.transform.forward,
                out RaycastHit hit, rayLength))
        {
            tagetName = hit.collider.gameObject;
            var readableItem = hit.collider.GetComponent<TextInspectItem>();
            if (readableItem != null)
            {
                textItem = readableItem;
                textItem.ShowObjectName(true);
                HighlightCrosshair(true);
            }
            else
            {
                ClearText();
            }
        }
        else
        {
            ClearText();
        }

        // 상호작용 키 입력 처리
        if (textItem != null)
        {
            if (Input.GetKeyDown(interactKey))
            {
                HandleInteraction(hit.collider.gameObject);
                textItem.ShowDetails();
            }
        }
    }

    void ClearText()
    {
        if (textItem != null)
        {
            textItem.ShowObjectName(false);
            HighlightCrosshair(false);
            textItem = null;
        }
    }

    void HighlightCrosshair(bool on)
    {
        TextInspectUIManager.instance.HighlightCrosshair(on);
    }

    void HandleInteraction(GameObject clickedObject)
    {
        var textInspectItem = clickedObject.GetComponent<TextInspectItem>();
        if (textInspectItem != null)
        {
            var player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
            if (player == null) return;

            if (textInspectItem.shoes)
            {
                HandleShoesInteraction(clickedObject, player);
            }
            else if (textInspectItem.gloves)
            {
                HandleGlovesInteraction(clickedObject, player);
            }
            else if (textInspectItem.heavy)
            {
                HandleHeavyInteraction(textInspectItem, player);
            }
            else if (textInspectItem.ismemory)
            {
                HandleMemoryInteractionServerRpc(clickedObject.GetComponent<NetworkObject>().NetworkObjectId);
            }
            else if (textInspectItem.isSpatialIntelligenceAbility)
            {
                HandleSpatialIntelligenceAbilityInteractionServerRpc(clickedObject.GetComponent<NetworkObject>()
                    .NetworkObjectId);
            }
            else if (textInspectItem.stone)
            {
                HandleStoneInteraction(clickedObject, player);
            }
        }
    }

    void HandleShoesInteraction(GameObject clickedObject, Player player)
    {
        var shoesScript = clickedObject.GetComponent<Shoes>();
        if (shoesScript != null)
        {
            shoesScript.GravitationCore = player.transform.GetChild(2).gameObject;
            if (shoesScript.GravitationCore != null)
            {
                shoesScript.Interaction();
            }
        }
    }

    void HandleGlovesInteraction(GameObject clickedObject, Player player)
    {
        var gloveScript = clickedObject.GetComponent<gloves>();
        if (gloveScript != null)
        {
            var skinnedMeshRenderer =
                player.transform.GetChild(1).GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                var materials = skinnedMeshRenderer.materials;
                if (materials.Length > 1)
                {
                    materials[1] = clickedObject.GetComponent<TextInspectItem>().material;
                    skinnedMeshRenderer.materials = materials;
                }
            }

            gloveScript.Interaction();
        }
    }

    void HandleHeavyInteraction(TextInspectItem textInspectItem, Player player)
    {
        if (player.transform.childCount > 4)
        {
            if (player.isgloves.Value)
            {
                textInspectItem.showObjectDetails = false;
                var networkObject = textInspectItem.GetComponent<NetworkObject>();
                if (networkObject != null && networkObject.IsSpawned)
                {
                    var prefabReference = new NetworkObjectReference(networkObject);
                    SpawnHeavyItemServerRpc(player.NetworkObjectId, prefabReference,
                        player.transform.GetChild(4).position, player.transform.GetChild(4).rotation);
                    DestroyItemServerRpc(networkObject.NetworkObjectId);
                    player.SetPickUp(true);
                }
            }
            else
            {
                textInspectItem.showObjectDetails = true;
                textInspectItem.objectDetails = "엄청무겁다.";
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnHeavyItemServerRpc(ulong playerNetworkObjectId, NetworkObjectReference prefabReference, Vector3 position,
        Quaternion rotation)
    {
        if (!prefabReference.TryGet(out var prefab))
        {
            return;
        }

        var instantiatedObject = Instantiate(prefab, position, rotation);
        var networkObject = instantiatedObject.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            Destroy(instantiatedObject);
            return;
        }

        instantiatedObject.GetComponent<Collider>().enabled = false;
        Destroy(instantiatedObject.GetComponent<Rigidbody>());
        networkObject.Spawn();

        UpdateParentServerRpc(networkObject.NetworkObjectId, playerNetworkObjectId);
    }


    [ServerRpc(RequireOwnership = false)]
    void UpdateParentServerRpc(ulong itemNetworkId, ulong parentNetworkId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkId, out var item) &&
            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(parentNetworkId, out var parent))
        {
            var parentTransform = parent.transform.GetChild(4).transform;
            var parentNetworkObject = parentTransform.GetComponentInParent<NetworkObject>();
            if (parentNetworkObject == null)
            {
                return;
            }

            item.transform.SetParent(parentNetworkObject.transform, true);
            UpdateParentClientRpc(itemNetworkId, parentNetworkId);
        }
    }

    [ClientRpc]
    void UpdateParentClientRpc(ulong itemNetworkId, ulong parentNetworkId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkId, out var item) &&
            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(parentNetworkId, out var parent))
        {
            var parentTransform = parent.transform.GetChild(4).transform;
            var parentNetworkObject = parentTransform.GetComponentInParent<NetworkObject>();
            if (parentNetworkObject == null)
            {
                return;
            }

            item.transform.SetParent(parentNetworkObject.transform, true);
        }

    }


    [ServerRpc(RequireOwnership = false)]
    void DestroyItemServerRpc(ulong networkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            Destroy(networkObject.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void HandleMemoryInteractionServerRpc(ulong clickedObjectNetworkId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(clickedObjectNetworkId,
                out var clickedObject))
        {
            var textInspectItem = clickedObject.GetComponent<TextInspectItem>();
            if (textInspectItem != null)
            {
                var player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
                if (player == null) return;

                if (textInspectItem.memorys.start)
                {
                    if (!MemoryStage.Instance.start.Value)
                    {
                        MemoryStage.Instance.MemoryStart();
                    }
                }
                else if (textInspectItem.memorys.reset)
                {
                    if (MemoryStage.Instance.start.Value)
                    {
                        MemoryStage.Instance.MemoryReset();
                    }
                }
                else if (MemoryStage.Instance.start.Value)
                {
                    MemoryStage.Instance.CheckColor(textInspectItem);
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void HandleSpatialIntelligenceAbilityInteractionServerRpc(ulong clickedObjectNetworkId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(clickedObjectNetworkId,
                out var clickedObject))
        {
            var textInspectItem = clickedObject.GetComponent<TextInspectItem>();
            if (textInspectItem != null)
            {
                var player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
                if (player == null) return;

                bool newValue = false;
                switch (textInspectItem.posX)
                {
                    case 0:
                        switch (textInspectItem.posY)
                        {
                            case 0:
                                newValue = !Spatialintelligence.Instance.poschecksX0.pos0;
                                Spatialintelligence.Instance.poschecksX0.pos0 = newValue;
                                break;
                            case 1:
                                newValue = !Spatialintelligence.Instance.poschecksX0.pos1;
                                Spatialintelligence.Instance.poschecksX0.pos1 = newValue;
                                break;
                            case 2:
                                newValue = !Spatialintelligence.Instance.poschecksX0.pos2;
                                Spatialintelligence.Instance.poschecksX0.pos2 = newValue;
                                break;
                            case 3:
                                newValue = !Spatialintelligence.Instance.poschecksX0.pos3;
                                Spatialintelligence.Instance.poschecksX0.pos3 = newValue;
                                break;
                            case 4:
                                newValue = !Spatialintelligence.Instance.poschecksX0.pos4;
                                Spatialintelligence.Instance.poschecksX0.pos4 = newValue;
                                break;
                            case 5:
                                newValue = !Spatialintelligence.Instance.poschecksX0.pos5;
                                Spatialintelligence.Instance.poschecksX0.pos5 = newValue;
                                break;
                            case 6:
                                newValue = !Spatialintelligence.Instance.poschecksX0.pos6;
                                Spatialintelligence.Instance.poschecksX0.pos6 = newValue;
                                break;
                        }

                        break;
                    case 1:
                        switch (textInspectItem.posY)
                        {
                            case 0:
                                newValue = !Spatialintelligence.Instance.poschecksX1.pos0;
                                Spatialintelligence.Instance.poschecksX1.pos0 = newValue;
                                break;
                            case 1:
                                newValue = !Spatialintelligence.Instance.poschecksX1.pos1;
                                Spatialintelligence.Instance.poschecksX1.pos1 = newValue;
                                break;
                            case 2:
                                newValue = !Spatialintelligence.Instance.poschecksX1.pos2;
                                Spatialintelligence.Instance.poschecksX1.pos2 = newValue;
                                break;
                            case 3:
                                newValue = !Spatialintelligence.Instance.poschecksX1.pos3;
                                Spatialintelligence.Instance.poschecksX1.pos3 = newValue;
                                break;
                            case 4:
                                newValue = !Spatialintelligence.Instance.poschecksX1.pos4;
                                Spatialintelligence.Instance.poschecksX1.pos4 = newValue;
                                break;
                            case 5:
                                newValue = !Spatialintelligence.Instance.poschecksX1.pos5;
                                Spatialintelligence.Instance.poschecksX1.pos5 = newValue;
                                break;
                            case 6:
                                newValue = !Spatialintelligence.Instance.poschecksX1.pos6;
                                Spatialintelligence.Instance.poschecksX1.pos6 = newValue;
                                break;
                        }

                        break;
                    case 2:
                        switch (textInspectItem.posY)
                        {
                            case 0:
                                newValue = !Spatialintelligence.Instance.poschecksX2.pos0;
                                Spatialintelligence.Instance.poschecksX2.pos0 = newValue;
                                break;
                            case 1:
                                newValue = !Spatialintelligence.Instance.poschecksX2.pos1;
                                Spatialintelligence.Instance.poschecksX2.pos1 = newValue;
                                break;
                            case 2:
                                newValue = !Spatialintelligence.Instance.poschecksX2.pos2;
                                Spatialintelligence.Instance.poschecksX2.pos2 = newValue;
                                break;
                            case 3:
                                newValue = !Spatialintelligence.Instance.poschecksX2.pos3;
                                Spatialintelligence.Instance.poschecksX2.pos3 = newValue;
                                break;
                            case 4:
                                newValue = !Spatialintelligence.Instance.poschecksX2.pos4;
                                Spatialintelligence.Instance.poschecksX2.pos4 = newValue;
                                break;
                            case 5:
                                newValue = !Spatialintelligence.Instance.poschecksX2.pos5;
                                Spatialintelligence.Instance.poschecksX2.pos5 = newValue;
                                break;
                            case 6:
                                newValue = !Spatialintelligence.Instance.poschecksX2.pos6;
                                Spatialintelligence.Instance.poschecksX2.pos6 = newValue;
                                break;
                        }

                        break;
                    case 3:
                        switch (textInspectItem.posY)
                        {
                            case 0:
                                newValue = !Spatialintelligence.Instance.poschecksX3.pos0;
                                Spatialintelligence.Instance.poschecksX3.pos0 = newValue;
                                break;
                            case 1:
                                newValue = !Spatialintelligence.Instance.poschecksX3.pos1;
                                Spatialintelligence.Instance.poschecksX3.pos1 = newValue;
                                break;
                            case 2:
                                newValue = !Spatialintelligence.Instance.poschecksX3.pos2;
                                Spatialintelligence.Instance.poschecksX3.pos2 = newValue;
                                break;
                            case 3:
                                newValue = !Spatialintelligence.Instance.poschecksX3.pos3;
                                Spatialintelligence.Instance.poschecksX3.pos3 = newValue;
                                break;
                            case 4:
                                newValue = !Spatialintelligence.Instance.poschecksX3.pos4;
                                Spatialintelligence.Instance.poschecksX3.pos4 = newValue;
                                break;
                            case 5:
                                newValue = !Spatialintelligence.Instance.poschecksX3.pos5;
                                Spatialintelligence.Instance.poschecksX3.pos5 = newValue;
                                break;
                            case 6:
                                newValue = !Spatialintelligence.Instance.poschecksX3.pos6;
                                Spatialintelligence.Instance.poschecksX3.pos6 = newValue;
                                break;
                        }

                        break;
                    case 4:
                        switch (textInspectItem.posY)
                        {
                            case 0:
                                newValue = !Spatialintelligence.Instance.poschecksX4.pos0;
                                Spatialintelligence.Instance.poschecksX4.pos0 = newValue;
                                break;
                            case 1:
                                newValue = !Spatialintelligence.Instance.poschecksX4.pos1;
                                Spatialintelligence.Instance.poschecksX4.pos1 = newValue;
                                break;
                            case 2:
                                newValue = !Spatialintelligence.Instance.poschecksX4.pos2;
                                Spatialintelligence.Instance.poschecksX4.pos2 = newValue;
                                break;
                            case 3:
                                newValue = !Spatialintelligence.Instance.poschecksX4.pos3;
                                Spatialintelligence.Instance.poschecksX4.pos3 = newValue;
                                break;
                            case 4:
                                newValue = !Spatialintelligence.Instance.poschecksX4.pos4;
                                Spatialintelligence.Instance.poschecksX4.pos4 = newValue;
                                break;
                            case 5:
                                newValue = !Spatialintelligence.Instance.poschecksX4.pos5;
                                Spatialintelligence.Instance.poschecksX4.pos5 = newValue;
                                break;
                            case 6:
                                newValue = !Spatialintelligence.Instance.poschecksX4.pos6;
                                Spatialintelligence.Instance.poschecksX4.pos6 = newValue;
                                break;
                        }

                        break;
                    case 5:
                        switch (textInspectItem.posY)
                        {
                            case 0:
                                newValue = !Spatialintelligence.Instance.poschecksX5.pos0;
                                Spatialintelligence.Instance.poschecksX5.pos0 = newValue;
                                break;
                            case 1:
                                newValue = !Spatialintelligence.Instance.poschecksX5.pos1;
                                Spatialintelligence.Instance.poschecksX5.pos1 = newValue;
                                break;
                            case 2:
                                newValue = !Spatialintelligence.Instance.poschecksX5.pos2;
                                Spatialintelligence.Instance.poschecksX5.pos2 = newValue;
                                break;
                            case 3:
                                newValue = !Spatialintelligence.Instance.poschecksX5.pos3;
                                Spatialintelligence.Instance.poschecksX5.pos3 = newValue;
                                break;
                            case 4:
                                newValue = !Spatialintelligence.Instance.poschecksX5.pos4;
                                Spatialintelligence.Instance.poschecksX5.pos4 = newValue;
                                break;
                            case 5:
                                newValue = !Spatialintelligence.Instance.poschecksX5.pos5;
                                Spatialintelligence.Instance.poschecksX5.pos5 = newValue;
                                break;
                            case 6:
                                newValue = !Spatialintelligence.Instance.poschecksX5.pos6;
                                Spatialintelligence.Instance.poschecksX5.pos6 = newValue;
                                break;
                        }

                        break;
                    case 6:
                        switch (textInspectItem.posY)
                        {
                            case 0:
                                newValue = !Spatialintelligence.Instance.poschecksX6.pos0;
                                Spatialintelligence.Instance.poschecksX6.pos0 = newValue;
                                break;
                            case 1:
                                newValue = !Spatialintelligence.Instance.poschecksX6.pos1;
                                Spatialintelligence.Instance.poschecksX6.pos1 = newValue;
                                break;
                            case 2:
                                newValue = !Spatialintelligence.Instance.poschecksX6.pos2;
                                Spatialintelligence.Instance.poschecksX6.pos2 = newValue;
                                break;
                            case 3:
                                newValue = !Spatialintelligence.Instance.poschecksX6.pos3;
                                Spatialintelligence.Instance.poschecksX6.pos3 = newValue;
                                break;
                            case 4:
                                newValue = !Spatialintelligence.Instance.poschecksX6.pos4;
                                Spatialintelligence.Instance.poschecksX6.pos4 = newValue;
                                break;
                            case 5:
                                newValue = !Spatialintelligence.Instance.poschecksX6.pos5;
                                Spatialintelligence.Instance.poschecksX6.pos5 = newValue;
                                break;
                            case 6:
                                newValue = !Spatialintelligence.Instance.poschecksX6.pos6;
                                Spatialintelligence.Instance.poschecksX6.pos6 = newValue;
                                break;
                        }

                        break;
                }

                textInspectItem.gameObject.GetComponent<Renderer>().material = newValue
                    ? Spatialintelligence.Instance.clickColor
                    : Spatialintelligence.Instance.defaultColor;

                UpdateClientRpc(clickedObjectNetworkId, textInspectItem.posX, textInspectItem.posY, newValue);
            }
        }
    }

    [ClientRpc]
    void UpdateClientRpc(ulong clickedObjectNetworkId, int posX, int posY, bool newValue)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(clickedObjectNetworkId,
                out var clickedObject))
        {
            var textInspectItem = clickedObject.GetComponent<TextInspectItem>();
            if (textInspectItem != null)
            {
                switch (posX)
                {
                    case 0:
                        switch (posY)
                        {
                            case 0:
                                Spatialintelligence.Instance.poschecksX0.pos0 = newValue;
                                break;
                            case 1:
                                Spatialintelligence.Instance.poschecksX0.pos1 = newValue;
                                break;
                            case 2:
                                Spatialintelligence.Instance.poschecksX0.pos2 = newValue;
                                break;
                            case 3:
                                Spatialintelligence.Instance.poschecksX0.pos3 = newValue;
                                break;
                            case 4:
                                Spatialintelligence.Instance.poschecksX0.pos4 = newValue;
                                break;
                            case 5:
                                Spatialintelligence.Instance.poschecksX0.pos5 = newValue;
                                break;
                            case 6:
                                Spatialintelligence.Instance.poschecksX0.pos6 = newValue;
                                break;
                        }

                        break;
                    case 1:
                        switch (posY)
                        {
                            case 0:
                                Spatialintelligence.Instance.poschecksX1.pos0 = newValue;
                                break;
                            case 1:
                                Spatialintelligence.Instance.poschecksX1.pos1 = newValue;
                                break;
                            case 2:
                                Spatialintelligence.Instance.poschecksX1.pos2 = newValue;
                                break;
                            case 3:
                                Spatialintelligence.Instance.poschecksX1.pos3 = newValue;
                                break;
                            case 4:
                                Spatialintelligence.Instance.poschecksX1.pos4 = newValue;
                                break;
                            case 5:
                                Spatialintelligence.Instance.poschecksX1.pos5 = newValue;
                                break;
                            case 6:
                                Spatialintelligence.Instance.poschecksX1.pos6 = newValue;
                                break;
                        }

                        break;
                    case 2:
                        switch (posY)
                        {
                            case 0:
                                Spatialintelligence.Instance.poschecksX2.pos0 = newValue;
                                break;
                            case 1:
                                Spatialintelligence.Instance.poschecksX2.pos1 = newValue;
                                break;
                            case 2:
                                Spatialintelligence.Instance.poschecksX2.pos2 = newValue;
                                break;
                            case 3:
                                Spatialintelligence.Instance.poschecksX2.pos3 = newValue;
                                break;
                            case 4:
                                Spatialintelligence.Instance.poschecksX2.pos4 = newValue;
                                break;
                            case 5:
                                Spatialintelligence.Instance.poschecksX2.pos5 = newValue;
                                break;
                            case 6:
                                Spatialintelligence.Instance.poschecksX2.pos6 = newValue;
                                break;
                        }

                        break;
                    case 3:
                        switch (posY)
                        {
                            case 0:
                                Spatialintelligence.Instance.poschecksX3.pos0 = newValue;
                                break;
                            case 1:
                                Spatialintelligence.Instance.poschecksX3.pos1 = newValue;
                                break;
                            case 2:
                                Spatialintelligence.Instance.poschecksX3.pos2 = newValue;
                                break;
                            case 3:
                                Spatialintelligence.Instance.poschecksX3.pos3 = newValue;
                                break;
                            case 4:
                                Spatialintelligence.Instance.poschecksX3.pos4 = newValue;
                                break;
                            case 5:
                                Spatialintelligence.Instance.poschecksX3.pos5 = newValue;
                                break;
                            case 6:
                                Spatialintelligence.Instance.poschecksX3.pos6 = newValue;
                                break;
                        }

                        break;
                    case 4:
                        switch (posY)
                        {
                            case 0:
                                Spatialintelligence.Instance.poschecksX4.pos0 = newValue;
                                break;
                            case 1:
                                Spatialintelligence.Instance.poschecksX4.pos1 = newValue;
                                break;
                            case 2:
                                Spatialintelligence.Instance.poschecksX4.pos2 = newValue;
                                break;
                            case 3:
                                Spatialintelligence.Instance.poschecksX4.pos3 = newValue;
                                break;
                            case 4:
                                Spatialintelligence.Instance.poschecksX4.pos4 = newValue;
                                break;
                            case 5:
                                Spatialintelligence.Instance.poschecksX4.pos5 = newValue;
                                break;
                            case 6:
                                Spatialintelligence.Instance.poschecksX4.pos6 = newValue;
                                break;
                        }

                        break;
                    case 5:
                        switch (posY)
                        {
                            case 0:
                                Spatialintelligence.Instance.poschecksX5.pos0 = newValue;
                                break;
                            case 1:
                                Spatialintelligence.Instance.poschecksX5.pos1 = newValue;
                                break;
                            case 2:
                                Spatialintelligence.Instance.poschecksX5.pos2 = newValue;
                                break;
                            case 3:
                                Spatialintelligence.Instance.poschecksX5.pos3 = newValue;
                                break;
                            case 4:
                                Spatialintelligence.Instance.poschecksX5.pos4 = newValue;
                                break;
                            case 5:
                                Spatialintelligence.Instance.poschecksX5.pos5 = newValue;
                                break;
                            case 6:
                                Spatialintelligence.Instance.poschecksX5.pos6 = newValue;
                                break;
                        }

                        break;
                    case 6:
                        switch (posY)
                        {
                            case 0:
                                Spatialintelligence.Instance.poschecksX6.pos0 = newValue;
                                break;
                            case 1:
                                Spatialintelligence.Instance.poschecksX6.pos1 = newValue;
                                break;
                            case 2:
                                Spatialintelligence.Instance.poschecksX6.pos2 = newValue;
                                break;
                            case 3:
                                Spatialintelligence.Instance.poschecksX6.pos3 = newValue;
                                break;
                            case 4:
                                Spatialintelligence.Instance.poschecksX6.pos4 = newValue;
                                break;
                            case 5:
                                Spatialintelligence.Instance.poschecksX6.pos5 = newValue;
                                break;
                            case 6:
                                Spatialintelligence.Instance.poschecksX6.pos6 = newValue;
                                break;
                        }

                        break;
                }

                textInspectItem.gameObject.GetComponent<Renderer>().material = newValue
                    ? Spatialintelligence.Instance.clickColor
                    : Spatialintelligence.Instance.defaultColor;
            }
        }
    }

    void HandleStoneInteraction(GameObject clickedObject, Player player)
    {
        var networkObject = clickedObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            HandleStoneInteractionServerRpc(networkObject.NetworkObjectId, player.NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void HandleStoneInteractionServerRpc(ulong clickedObjectNetworkId, ulong playerNetworkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(clickedObjectNetworkId,
                out var clickedObject))
        {
            TextInspectItem textInspectItem = clickedObject.GetComponent<TextInspectItem>();
            var networkObject = textInspectItem.GetComponent<NetworkObject>();
            var player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkObjectId]
                .GetComponent<Player>();

            if (textInspectItem.stoneNumber == 1)
            {
                CollectionOfSculptures.Instance.stone1 = true;
            }
            else if (textInspectItem.stoneNumber == 2)
            {
                CollectionOfSculptures.Instance.stone2 = true;
            }
            else if (textInspectItem.stoneNumber == 3)
            {
                CollectionOfSculptures.Instance.stone3 = true;
            }
            else if (textInspectItem.stoneNumber == 4)
            {
                CollectionOfSculptures.Instance.stone4 = true;
            }

            UpdateStoneClientRpc(textInspectItem.stoneNumber, clickedObjectNetworkId);
        }
    }

    [ClientRpc]
    void UpdateStoneClientRpc(int stoneNumber, ulong clickedObjectNetworkId)
    {
        switch (stoneNumber)
        {
            case 1:
                CollectionOfSculptures.Instance.stone1 = true;
                break;
            case 2:
                CollectionOfSculptures.Instance.stone2 = true;
                break;
            case 3:
                CollectionOfSculptures.Instance.stone3 = true;
                break;
            case 4:
                CollectionOfSculptures.Instance.stone4 = true;
                break;
        }

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(clickedObjectNetworkId,
                out var networkObject))
        {
            Destroy(networkObject.gameObject);
        }
    }
}
