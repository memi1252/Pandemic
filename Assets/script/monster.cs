using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Monster : NetworkBehaviour
{
    [SerializeField] private float followRange = 10f;
    [SerializeField] private GameObject monsterRange;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask notLayerMask;
    [SerializeField] float nearRadius = 0f;
    [SerializeField] private float hitRange = 1f;
    [SerializeField] public Transform nearGameObjectTansform;
    private NavMeshAgent navMeshAgent;
    private Vector3 initialPosition;
    private Transform player;
    private bool look;
    private bool following;
    private int lookAt;
    public Collider[] colliders;
    float distanceToPlayer;

    private bool dd;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;
        if (nearGameObjectTansform == null)
        {
            nearGameObjectTansform = this.transform;
        }
    }

    private void Start()
    {
        StartCoroutine(rotates());
    }

    void Update()
    {
        if (IsServer)
        {
            
            ServerUpdate();
        }
    }

    private void ServerUpdate()
    {
        colliders = Physics.OverlapSphere(monsterRange.transform.position, followRange, playerLayer);

        if (colliders.Length > 0)
        {
            if (colliders[0].gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if(!colliders[0].transform.parent.parent.GetComponent<Player>().isDie.Value)
                {
                    nearGameObjectTansform = colliders[0].transform;
                    distanceToPlayer = Vector3.Distance(monsterRange.transform.position, nearGameObjectTansform.position);
                    dd = true;
                }
            }
        }

        float distanceToMonster = Vector3.Distance(transform.position, monsterRange.transform.position);

        Debug.DrawRay(transform.position, transform.forward * nearRadius, Color.green);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, nearRadius))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if(!hit.collider.transform.parent.parent.GetComponent<Player>().isDie.Value)
                {
                    if (following)
                    {
                        distanceToPlayer = Vector3.Distance(hit.transform.position, monsterRange.transform.position);
                        nearGameObjectTansform = hit.transform;
                        look = true;
                        dd=true;
                    }
                }
            }
        }

        if (dd)
        {
            if (distanceToPlayer <= followRange)
            {
                navMeshAgent.SetDestination(nearGameObjectTansform.position);
                following = true;
            }
            else
            {
                navMeshAgent.SetDestination(initialPosition);
                following = false;
                dd=false;
            }
        }
        
        if (transform.position == initialPosition)
        {
            nearGameObjectTansform = null;
            following = false;
        }
        
        
        if (nearGameObjectTansform == transform)
        {
            following = false;
        }
        
        if (!following)
        {
            Vector3 direction =
                new Vector3(Mathf.Sin(lookAt * Mathf.Deg2Rad), 0, Mathf.Cos(lookAt * Mathf.Deg2Rad));
            Quaternion newRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 2);
        }
        
        UpdateClientRpc(transform.position, transform.rotation);
    }

    [ClientRpc]
    private void UpdateClientRpc(Vector3 position, Quaternion rotation)
    {
        if (IsOwner) return;

        transform.position = position;
        transform.rotation = rotation;
    }

    IEnumerator rotates()
    {
        yield return new WaitForSeconds(5f);
        lookAt = Random.Range(-180, 181);
        StartCoroutine(rotates());
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(monsterRange.transform.position, followRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, nearRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null && IsClient)
            {
                if (player.isDie.Value)
                {
                    navMeshAgent.SetDestination(initialPosition);
                    following = false;
                    look = false;
                    nearGameObjectTansform = null;
                    colliders = null;
                    distanceToPlayer = followRange + 1;
                    return;
                }
                player.TakeDamage(Random.Range(3, 5));
            }
        }
    }
}