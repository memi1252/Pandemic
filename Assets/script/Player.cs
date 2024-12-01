using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private bool isShoesInspector;

    public bool up = false;
    public bool down = false;
    public bool left= false;
    public bool right= false;
    public bool forward= false;
    public bool back= false;

    private bool kickCheeck = true;

    public int playerHP = 100;

    private Vector2 moveInput;

    private NetworkVariable<float> jumpForce = new NetworkVariable<float>(5f);
    private NetworkVariable<bool> isGrounded = new NetworkVariable<bool>(true);
    private NetworkVariable<bool> isWalking = new NetworkVariable<bool>();
    private NetworkVariable<bool> isJumping = new NetworkVariable<bool>();
    private NetworkVariable<bool> isKicking = new NetworkVariable<bool>();
    public NetworkVariable<bool> isshoes = new NetworkVariable<bool>(false);
    private NetworkVariable<Vector3> gravity = new NetworkVariable<Vector3>(new Vector3(0, -9.8f, 0));

    private Animator playerAnim;

    private void Awake()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody>();
        playerAnim = playerAnimGameObject.GetComponent<Animator>();
        playerAnim.applyRootMotion = false; // Disable root motion
        isshoes.OnValueChanged += OnShoesChanged;
    }

    private void Start()
    {
        if (IsServer)
        {
            gravity.Value = new Vector3(0, -9.8f, 0); // Set the gravity on the server
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        HandleInput();
        CheckGrounded();
    }

    private void HandleInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool jumpInput = Input.GetKeyDown(KeyCode.Space);
        bool kickInput = Input.GetKeyDown(KeyCode.Mouse1);
        MoveServerRpc(horizontalInput, verticalInput, jumpInput, kickInput);
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
    private void MoveServerRpc(float horizontalInput, float verticalInput, bool jumpInput, bool kickInput)
    {
        var moveDir = new Vector3(horizontalInput, 0, verticalInput);
        MoveClientRpc(moveDir, jumpInput, kickInput, jumpForce.Value, gravity.Value);
        UpdateAnimationState(horizontalInput, verticalInput, jumpInput, kickInput);
    }

    [ClientRpc]
    private void MoveClientRpc(Vector3 moveDir, bool jumpInput, bool kickInput, float syncedJumpForce, Vector3 syncedGravity)
    {
        Physics.gravity = syncedGravity; // Apply synchronized gravity

        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

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

    private void UpdateAnimationState(float horizontalInput, float verticalInput, bool jumpInput, bool kickInput)
    {
        isWalking.Value = horizontalInput != 0 || verticalInput != 0;
        isJumping.Value = !isGrounded.Value;

        UpdateAnimationClientRpc(isWalking.Value, isJumping.Value, isKicking.Value);
    }

    [ClientRpc]
    private void UpdateAnimationClientRpc(bool walking, bool jumping, bool kicking)
    {
        playerAnim.SetBool("isWalk", walking);
        playerAnim.SetBool("isjump", jumping);
        playerAnim.SetBool("iskick", kicking);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyGroundedServerRpc()
    {
        UpdateAnimationState(0, 0, false, false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyJumpedServerRpc()
    {
        UpdateAnimationState(0, 0, true, false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyKickingServerRpc(bool isKicking)
    {
        this.isKicking.Value = isKicking;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            hpBarUI = FindObjectOfType<HPBarUI>();
            hpBarUI.SetPlayer(this);
        }
    }

    public void TakeDamage(int damage)
    {
        playerHP -= damage;
        if (IsOwner)
        {
            hpBarUI.UpdateHP(playerHP);
        }
    }
    
    private void OnShoesChanged(bool oldValue, bool newValue)
    {
        isShoesInspector = newValue; // Update the Inspector variable
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, kickRange);
    }
}