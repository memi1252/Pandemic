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
    [SerializeField] private LayerMask wall;
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private float kickForce = 5f;
    [SerializeField] private float kickRange = 10f;

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
    private NetworkVariable<Vector3> gravity = new NetworkVariable<Vector3>(new Vector3(0, -9.8f, 0));

    private Animator playerAnim;

    private void Awake()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody>();
        playerAnim = playerGameObject.GetComponent<Animator>();
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
    }

    private void HandleInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool jumpInput = Input.GetKeyDown(KeyCode.Space);
        bool kickInput = Input.GetKeyDown(KeyCode.Mouse1);
        MoveServerRpc(horizontalInput, verticalInput, jumpInput, kickInput);
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
        
        Physics.gravity = syncedGravity; // Apply the synchronized gravity

        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

        if (jumpInput && isGrounded.Value)
        {
            rigidbody.linearVelocity = Vector3.zero; // Reset velocity to ensure consistent jump height
            rigidbody.AddForce(Vector3.up * syncedJumpForce, ForceMode.Impulse);
            NotifyJumpedServerRpc();
        }
        
        if(isGrounded.Value)
        {
            kickCheeck = true;
        }

        if (kickInput && !isGrounded.Value && kickCheeck)
        {
            Kick();
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
                UpdateAnimationClientRpc(false, false, false);
            }
        }
    }

    private void UpdateAnimationState(float horizontalInput, float verticalInput, bool jumpInput, bool kickInput)
    {
        isWalking.Value = horizontalInput != 0 || verticalInput != 0;
        isJumping.Value = !isGrounded.Value;
        isKicking.Value = kickInput && !isGrounded.Value;

        UpdateAnimationClientRpc(isWalking.Value, isJumping.Value, isKicking.Value);
    }

    [ClientRpc]
    private void UpdateAnimationClientRpc(bool walking, bool jumping, bool kicking)
    {
        playerAnim.SetBool("isWalk", walking);
        playerAnim.SetBool("isjump", jumping);
        playerAnim.SetBool("iskick", kicking);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            NotifyGroundedServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyGroundedServerRpc()
    {
        isGrounded.Value = true;
        UpdateAnimationState(0, 0, false, false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyJumpedServerRpc()
    {
        isGrounded.Value = false;
        UpdateAnimationState(0, 0, true, false);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, kickRange);

       
    }
}