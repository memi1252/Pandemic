using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TextInspectSystem;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    public static Player Instance { get; private set;}

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private GameObject playerGameObject;
    [SerializeField] private GameObject playerAnimGameObject;
    [SerializeField] private LayerMask wall;
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private float kickForce = 5f;
    [SerializeField] private float kickRange = 10f;
    [SerializeField] private HPBarUI hpBarUI;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Vector3[] spawnPoints;
    [SerializeField] private AudioSource audioSource; // Add this line
    [SerializeField] public TextMeshProUGUI playerNameText;
    
    

    public bool up = false;
    public bool down = false;
    public bool left= false;
    public bool right= false;
    public bool forward= false;
    public bool back= false;

    private bool kickCheeck = true;
    public bool playermove = true;

    public NetworkVariable<int> playerHP = new NetworkVariable<int>(100);
    private Vector2 moveInput;

    private NetworkVariable<float> jumpForce = new NetworkVariable<float>(5f);
    private NetworkVariable<bool> isGrounded = new NetworkVariable<bool>(true);
    private NetworkVariable<bool> isWalking = new NetworkVariable<bool>();
    private NetworkVariable<bool> isJumping = new NetworkVariable<bool>();
    private NetworkVariable<bool> isKicking = new NetworkVariable<bool>();
    private NetworkVariable<bool> isRunning = new NetworkVariable<bool>();
    public NetworkVariable<bool> isshoes = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isgloves = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isDie = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> ispickUpitme = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> ispickUpStone1 = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> ispickUpStone2 = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> ispickUpStone3 = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> ispickUpStone4 = new NetworkVariable<bool>(false);

    private NetworkVariable<Vector3> gravity = new NetworkVariable<Vector3>(new Vector3(0, -9.8f, 0));

    private Animator playerAnim;

    private string[] playername = new string[4]; 
    
    
    private void Awake()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody>();
        playerAnim = playerAnimGameObject.GetComponent<Animator>();
        playerAnim.applyRootMotion = false; // Disable root motion
        isshoes.OnValueChanged += OnShoesChanged;
        playerHP.OnValueChanged += OnHPChanged;
    }

    private void Start()
    {
        if (IsServer)
        {
            gravity.Value = new Vector3(0, -9.8f, 0); // Set the gravity on the server
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.spatialBlend = 0.5f; // Set to 3D sound
        }
        
        NetworkManager.Singleton.SpawnManager.PlayerObjects[(int)NetworkManager.Singleton.LocalClientId].GetComponent<Player>().playerNameText.text = LobbyMake.Instance.playerNameInput.text;
        switch ((int)(NetworkManager.Singleton.LocalClientId))
        {
            case 1:
                playername[0] = LobbyMake.Instance.playerNameInput.text;
                break;
            case 2:
                playername[1] = LobbyMake.Instance.playerNameInput.text;
                break;
            case 3:
                playername[2] = LobbyMake.Instance.playerNameInput.text;
                break;
            case 4:
                playername[3] = LobbyMake.Instance.playerNameInput.text;
                break;
        }
        NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManagerOnOnLoadComplete;
    }
    
    void SceneManagerOnOnLoadComplete(ulong clientid, string scenename, LoadSceneMode loadscenemode)
    {
        if (scenename == "gameScene")
        {
            UpdatePlayerPositionsClientRpc();
        }
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= SceneManagerOnOnLoadComplete;
    }
    
    [ClientRpc]
    private void UpdatePlayerPositionsClientRpc()
    {
        var localClientId = NetworkManager.Singleton.LocalClientId;
        if (localClientId == 0)
        {
            transform.position = new Vector3(2, 2, 387);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (localClientId == 1)
        {
            transform.position = new Vector3(0, 2, 397);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (localClientId == 2)
        {
            transform.position = new Vector3(-2, 2, 397);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (localClientId == 3)
        {
            transform.position = new Vector3(-4, 2, 397);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!playermove) return;

        if (hpBarUI == null)
        {
            hpBarUI = FindObjectOfType<HPBarUI>();
            if (hpBarUI != null)
            {
                hpBarUI.SetPlayer(this);
            }
        }

        HandleInput();
        CheckGrounded();
    }

    private void HandleInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool jumpInput = Input.GetKeyDown(KeyCode.Space);
        bool kickInput = Input.GetKeyDown(KeyCode.Mouse1);
        bool runInput = Input.GetKey(KeyCode.LeftShift);
        bool itmeDrop = Input.GetKeyDown(KeyCode.Backspace);

        MoveServerRpc(horizontalInput, verticalInput, jumpInput, kickInput, runInput, itmeDrop);
        
    }

    private void CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, groundCheckDistance))
        {
            if (!isGrounded.Value)
            {
                UpdateGroundedStatusServerRpc(true);
            }
        }
        else
        {
            if (isGrounded.Value)
            {
                UpdateGroundedStatusServerRpc(false);
            }
        }
    }

    [ServerRpc]
    private void UpdateGroundedStatusServerRpc(bool grounded)
    {
        isGrounded.Value = grounded;
        if (grounded)
        {
            NotifyGroundedServerRpc();
        }
        else
        {
            NotifyJumpedServerRpc();
        }
    }

    [ServerRpc]
    private void MoveServerRpc(float horizontalInput, float verticalInput, bool jumpInput, bool kickInput, bool runInput, bool itmeDrop)
    {
        var moveDir = new Vector3(horizontalInput, 0, verticalInput);

        MoveClientRpc(moveDir, jumpInput, kickInput, jumpForce.Value, gravity.Value, runInput, itmeDrop);
        UpdateAnimationState(horizontalInput, verticalInput, jumpInput, kickInput, runInput);
    }

    [ClientRpc]
    private void MoveClientRpc(Vector3 moveDir, bool jumpInput, bool kickInput, float syncedJumpForce, Vector3 syncedGravity, bool runInput, bool itmeDrop)
    {
        Physics.gravity = syncedGravity; // Apply synchronized gravity

        if (runInput)
        {
            transform.Translate(moveDir * (moveSpeed + 3) * Time.deltaTime);
        }
        else
        {
            transform.Translate(moveDir * moveSpeed * Time.deltaTime);
        }

        if (itmeDrop)
        {
            if (IsOwner && ispickUpitme.Value)
            {
                DropItem();
            }
        }

        if (jumpInput && isGrounded.Value)
        {
            rigidbody.linearVelocity = Vector3.zero; // Reset velocity to ensure consistent jump height
            rigidbody.AddForce(transform.up * syncedJumpForce, ForceMode.Impulse); // Apply jump force in the upward direction
            NotifyJumpedServerRpc();
        }

        if (kickInput && !isGrounded.Value)
        {
            Kick();
            NotifyKickingServerRpc(true);
        }
        else
        {
            NotifyKickingServerRpc(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnAndDropItemServerRpc(NetworkObjectReference itemPrefabRef, Vector3 position, Quaternion rotation)
    {
        // 네트워크 오브젝트 참조 가져오기
        if (!itemPrefabRef.TryGet(out NetworkObject prefab))
        {
            Debug.LogError("Failed to get prefab network object");
            return;
        }

        // 프리팹 인스턴스화
        var instantiatedObject = Instantiate(prefab.gameObject, position, rotation);
        instantiatedObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        var itemCollider = instantiatedObject.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
            var playerCollider = GetComponent<Collider>();

            if (playerCollider != null)
            {
                Physics.IgnoreCollision(itemCollider, playerCollider, true);
            }
        }

        instantiatedObject.AddComponent<Rigidbody>();

        // 네트워크 오브젝트 스폰
        var networkObject = instantiatedObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn(true); // true means it's owned by the server
        }
        else
        {
            Debug.LogError("No NetworkObject component found on instantiated object");
            Destroy(instantiatedObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnItemServerRpc()
    {
        var networkObject = transform.GetChild(5).GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Despawn();
        }
    }

    private void DropItem()
    {
        var textInspectInteractor = transform.GetChild(5).GetComponent<TextInspectItem>();

        // Convert prefab to NetworkObjectReference
        NetworkObjectReference prefabRef = textInspectInteractor.Prefabs.GetComponent<NetworkObject>();

        // Request server to update the stone pick-up status
        UpdatePickUpStoneServerRpc(NetworkObjectId, textInspectInteractor.stoneNumber, false);

        // Enable collider and restore collision
        var itemCollider = textInspectInteractor.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
            var playerCollider = GetComponent<Collider>();
            if (playerCollider != null)
            {
                Physics.IgnoreCollision(itemCollider, playerCollider, false);
            }
        }

        // Request server to spawn the item
        SpawnAndDropItemServerRpc(
            prefabRef,
            transform.GetChild(4).position,
            transform.GetChild(4).rotation
        );

        SetPickUp(false);

        // Request server to despawn the item
        DespawnItemServerRpc();
    }

    private void Kick()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, kickRange, monsterLayer);
        foreach (var hitCollider in hitColliders)
        {
            Rigidbody monsterRigidbody = hitCollider.GetComponent<Rigidbody>();
            if (monsterRigidbody != null)
            {
                Vector3 direction = (hitCollider.transform.position - transform.position).normalized;
                monsterRigidbody.AddForce(direction * kickForce, ForceMode.Impulse);
                kickCheeck = false;
            }
        }
    }
    
    [ClientRpc]
    private void PlayWalkingSoundClientRpc(ulong playerId)
    {
        if (NetworkManager.Singleton.LocalClientId != playerId)
        {
            SoundManager.Instance.PlayWalkingSound(audioSource);
        }
    }

    [ClientRpc]
    private void StopWalkingSoundClientRpc(ulong playerId)
    {
        if (NetworkManager.Singleton.LocalClientId != playerId)
        {
            SoundManager.Instance.StopWalkingSound(audioSource);
        }
    }

    [ClientRpc]
    private void PlayRunningSoundClientRpc(ulong playerId)
    {
        if (NetworkManager.Singleton.LocalClientId != playerId)
        {
            SoundManager.Instance.PlayRunningSound(audioSource);
        }
    }

    [ClientRpc]
    private void StopRunningSoundClientRpc(ulong playerId)
    {
        if (NetworkManager.Singleton.LocalClientId != playerId)
        {
            SoundManager.Instance.StopRunningSound(audioSource);
        }
    }

    public void UpdateAnimationState(float horizontalInput, float verticalInput, bool jumpInput, bool kickInput, bool runInput)
    {
        isWalking.Value = horizontalInput != 0 || verticalInput != 0;
        isJumping.Value = !isGrounded.Value;
        isRunning.Value = (horizontalInput != 0 || verticalInput != 0) && runInput;
        if (!isDie.Value)
        {
            if (playerHP.Value <= 0)
            {
                Debug.Log("죽음");
                isDie.Value = true;
                playermove = false;
                DisableColliders();
            }
        }

        if (isWalking.Value && !isRunning.Value)
        {
            if (IsOwner)
            {
                SoundManager.Instance.StopRunningSound(audioSource);
                SoundManager.Instance.PlayWalkingSound(audioSource);
            }
            StopRunningSoundClientRpc(NetworkManager.Singleton.LocalClientId);
            PlayWalkingSoundClientRpc(NetworkManager.Singleton.LocalClientId);
        }
        else if (isWalking.Value && isRunning.Value)
        {
            if (IsOwner)
            {
                SoundManager.Instance.StopWalkingSound(audioSource);
                SoundManager.Instance.PlayRunningSound(audioSource);
            }
            StopWalkingSoundClientRpc(NetworkManager.Singleton.LocalClientId);
            PlayRunningSoundClientRpc(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            if (IsOwner)
            {
                SoundManager.Instance.StopWalkingSound(audioSource);
                SoundManager.Instance.StopRunningSound(audioSource);
            }
            StopWalkingSoundClientRpc(NetworkManager.Singleton.LocalClientId);
            StopRunningSoundClientRpc(NetworkManager.Singleton.LocalClientId);
        }

        UpdateAnimationClientRpc(isWalking.Value, isJumping.Value, isKicking.Value, isRunning.Value, isDie.Value);
    }
    
    private void DisableColliders()
    {
        // Disable all colliders on the player
        Collider capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        Collider boxCollider = GetComponentInChildren<BoxCollider>();
        TextInspectItem textInspectItem = GetComponentInChildren<TextInspectItem>();
        capsuleCollider.enabled = false;
        boxCollider.enabled = true;
        textInspectItem.enabled = true;
        textInspectItem.showObjectName = true;

    }

    [ClientRpc]
    private void UpdateAnimationClientRpc(bool walking, bool jumping, bool kicking, bool running, bool die)
    {
        playerAnim.SetBool("isWalk", walking);
        playerAnim.SetBool("isjump", jumping);
        playerAnim.SetBool("iskick", kicking);
        playerAnim.SetBool("isRun", running);
        playerAnim.SetBool("isDie", die);
        
        if (die)
        {
            playermove = false;
            DisableColliders();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyGroundedServerRpc()
    {
        UpdateAnimationState(0, 0, false, false, false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyJumpedServerRpc()
    {
        UpdateAnimationState(0, 0, true, false, false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyKickingServerRpc(bool isKicking)
    {
        this.isKicking.Value = isKicking;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            // Host spawns at spawnPoints[0]
            
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsServer)
        {
            playerHP.Value -= damage;
        }
    }

    private void OnHPChanged(int oldValue, int newValue)
    {
        if (IsOwner && hpBarUI != null)
        {
            hpBarUI.UpdateHP(newValue);
        }
    }

    private void OnShoesChanged(bool oldValue, bool newValue)
    {
    }

    public void SetShoes(bool value)
    {
        if (IsServer)
        {
            isshoes.Value = value;
        }
        else
        {
            SetShoesServerRpc(value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetShoesServerRpc(bool value)
    {
        isshoes.Value = value;
    }

    public void SetGlove(bool value)
    {
        if (IsServer)
        {
            isgloves.Value = value;
        }
        else
        {
            SetGloveServerRpc(value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetGloveServerRpc(bool value)
    {
        isgloves.Value = value;
    }

    public void SetPickUp(bool value)
    {
        if (IsServer)
        {
            ispickUpitme.Value = value;
        }
        else
        {
            SetPickUpServerRpc(value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPickUpServerRpc(bool value)
    {
        ispickUpitme.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnAndCaseItemServerRpc(NetworkObjectReference itemPrefabRef, Vector3 position, Quaternion rotation)
    {
        // 네트워크 오브젝트 참조 가져오기
        if (!itemPrefabRef.TryGet(out NetworkObject prefab))
        {
            Debug.LogError("Failed to get prefab network object");
            return;
        }

        // 프리팹 인스턴스화
        var instantiatedObject = Instantiate(prefab.gameObject, position, rotation);
        instantiatedObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        instantiatedObject.GetComponent<Collider>().enabled = true;
        instantiatedObject.GetComponent<TextInspectItem>().enabled = true;
        var itemCollider = instantiatedObject.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
            var playerCollider = GetComponent<Collider>();

            if (playerCollider != null)
            {
                Physics.IgnoreCollision(itemCollider, playerCollider, true);
            }
        }

        // 네트워크 오브젝트 스폰
        var networkObject = instantiatedObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn(true); // true means it's owned by the server
        }
        else
        {
            Debug.LogError("No NetworkObject component found on instantiated object");
            Destroy(instantiatedObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnItemCaseServerRpc()
    {
        var networkObject = transform.GetChild(5).GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Despawn();
        }
    }

    public void ItemPutCase(GameObject clickobject)
    {
        var textInspectInteractor = transform.GetChild(5).GetComponent<TextInspectItem>();

        // Convert prefab to NetworkObjectReference
        NetworkObjectReference prefabRef = textInspectInteractor.Prefabs.GetComponent<NetworkObject>();

        if (textInspectInteractor.stoneNumber == 1)
        {
            UpdatePickUpStoneServerRpc(NetworkObjectId, 1, false);
        }
        else if (textInspectInteractor.stoneNumber == 2)
        {
            UpdatePickUpStoneServerRpc(NetworkObjectId, 2, false);
        }
        else if (textInspectInteractor.stoneNumber == 3)
        {
            UpdatePickUpStoneServerRpc(NetworkObjectId, 3, false);
        }
        else if (textInspectInteractor.stoneNumber == 4)
        {
            UpdatePickUpStoneServerRpc(NetworkObjectId, 4, false);
        }

        // Request server to spawn the item
        SpawnAndCaseItemServerRpc(
            prefabRef,
            clickobject.transform.GetChild(0).position,
            clickobject.transform.GetChild(0).rotation
        );

        SetPickUp(false);

        // Request server to despawn the item
        DespawnItemCaseServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdatePickUpStoneServerRpc(ulong playerNetworkObjectId, int stoneNumber, bool value)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerNetworkObjectId, out var playerObject))
        {
            var player = playerObject.GetComponent<Player>();
            if (player != null)
            {
                switch (stoneNumber)
                {
                    case 1:
                        player.ispickUpStone1.Value = value;
                        break;
                    case 2:
                        player.ispickUpStone2.Value = value;
                        break;
                    case 3:
                        player.ispickUpStone3.Value = value;
                        break;
                    case 4:
                        player.ispickUpStone4.Value = value;
                        break;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, kickRange);
    }
}