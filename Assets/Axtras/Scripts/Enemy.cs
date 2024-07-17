using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    internal Transform player;

    internal Animator animator;
    internal NavMeshAgent navMeshAgent;

    internal Vector3 originalPosition;
    internal Vector3 directionToPlayer;
    internal Vector3 lastSeenPosition;
    
    internal float distanceToPlayer;
    internal float angleToPlayer;
    internal float distanceToOriginalPosition;
    internal float distanceToLastSeenPosition;
    
    internal bool isIdle;
    internal bool isWalking;
    internal bool isChasing;
    internal bool isSearching;
    internal bool isAttacking;

    protected virtual void Start() {
        player = GameObject.Find("Player").transform;

        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        originalPosition = transform.position;
    }

    protected virtual void Update() {
        CalcValues();
    }

    private void CalcValues() {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
        directionToPlayer = (player.position - transform.position).normalized;
        angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        distanceToOriginalPosition = Vector3.Distance(transform.position, originalPosition);
        distanceToLastSeenPosition = Vector3.Distance(transform.position, lastSeenPosition);
    }
}