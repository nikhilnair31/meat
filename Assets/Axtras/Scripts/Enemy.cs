using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Transform player;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private Vector3 originalPosition;
    private Vector3 directionToPlayer;
    private Vector3 lastSeenPosition;
    private float resetTimer;
    private float searchingForTimer;
    private float distanceToPlayer;
    private float angleToPlayer;
    private float distanceToOriginalPosition;
    private float distanceToLastSeenPosition;
    private bool isIdle;
    public bool IsIdle {
        get { 
            return isIdle; 
        }
        set {
            if (isIdle != value) {
                isIdle = value;
                animator.SetBool("isIdle", value);
            }
            IsWalking = false;
        }
    }
    private bool isWalking;
    public bool IsWalking {
        get { 
            return isWalking; 
        }
        set {
            if (isWalking != value) {
                isWalking = value;
                animator.SetBool("isWalking", value);
            }
        }
    }
    private bool isChasing;
    public bool IsChasing {
        get { 
            return isChasing; 
        }
        set {
            if (isChasing != value) {
                isChasing = value;
                animator.SetBool("isChasing", value);
            }
            IsIdle = false;
        }
    }
    private bool isSearching;
    public bool IsSearching {
        get { 
            return isSearching; 
        }
        set {
            if (isSearching != value) {
                isSearching = value;
                animator.SetBool("isSearching", value);
            }
            IsChasing = false;
        }
    }
    private bool hasLostSight;
    public bool HasLostSight {
        get { 
            return hasLostSight; 
        }
        set {
            if (hasLostSight != value) {
                hasLostSight = value;
                animator.SetBool("hasLostSight", value);
            }
        }
    }

    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private float detectionAngle = 45f;
    [SerializeField] private float detectionDistance = 10f;
    [SerializeField] private float chaseDistance = 15f;
    [SerializeField] private float resetAfterDuration = 10f;
    [SerializeField] private float searchingForDuration = 5f;
    [SerializeField] private float waitTimeAtLastSeen = 3f;

    // FIXME: Fix enemy logic
    void Start() {
        player = GameObject.Find("Player").transform;

        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        originalPosition = transform.position;
        searchingForTimer = 0f;
        resetTimer = 0f;

        IsIdle = true;
        IsChasing = false;
        HasLostSight = false;
    }

    private void Update() {
        CalcValues();
        Chasing();
        Idle();
        Searching();
        Reseter();
    }

    private void CalcValues() {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
        directionToPlayer = (player.position - transform.position).normalized;
        angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        distanceToOriginalPosition = Vector3.Distance(transform.position, originalPosition);
        distanceToLastSeenPosition = Vector3.Distance(transform.position, lastSeenPosition);
    }

    private void Chasing() {
        // If not current chasing the player
        if (!IsChasing) {
            // If player in detetcion distance and angle range
            if (distanceToPlayer < detectionDistance && angleToPlayer < detectionAngle) {
                // Check if anything between enemy and player
                if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionDistance, detectionLayer)) {
                    // If can see player then chase
                    if (hit.transform.CompareTag("Player")) {
                        StartChasing();
                    }
                    // Something's between enemy and player so don't react
                    else {
                        //
                    }
                }
                // Raycats isn't hitting anything so don't react
                else {
                    //
                }
            }
        }
        // If enemy is currently chasing the player
        else {
            // If player in chase distance
            if (distanceToPlayer < chaseDistance) {
                // Check if anything between enemy and player
                if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, chaseDistance, detectionLayer)) {
                    // If can see player then chase
                    if (hit.transform.CompareTag("Player")) {
                        StartChasing();
                    }
                    // Something's between enemy and player so move to last seen position of player
                    else {
                        MoveToPlayerLastSeenPosition();
                    }
                }
                // Raycats isn't hitting anything so don't react
                else {
                    //
                }
            }
            // If player is out of chase distance then return to original position
            else {
                ReturnToOriginalPosition();
            }
        }
    }
    private void StartChasing() {
        IsChasing = true;

        lastSeenPosition = Helper.GetClosestPointOnNavMesh(player.position, 5f);
        navMeshAgent.SetDestination(lastSeenPosition);
    }
    private void MoveToPlayerLastSeenPosition() {
        IsChasing = true;

        navMeshAgent.SetDestination(lastSeenPosition);
    }
    private void ReturnToOriginalPosition() {
        IsChasing = false;
        IsWalking = true;

        navMeshAgent.SetDestination(originalPosition);
    }

    private void Idle() {
        if(!IsIdle) {
            if (distanceToOriginalPosition < 1.1f) {
                IsIdle = true;
            }
        }
    }

    private void Searching() {
        if(!IsSearching) {
            if (distanceToLastSeenPosition < 1.1f) {
                IsSearching = true;
            }
        }
        else {
            searchingForTimer += Time.deltaTime;
            if (searchingForTimer > searchingForDuration) {
                IsSearching = false;
                ReturnToOriginalPosition();
            }
        }
    }

    private void Reseter() {
        if(navMeshAgent.velocity.magnitude < 0.1f) {
            resetTimer += Time.deltaTime;
            if (resetTimer > resetAfterDuration) {
                IsSearching = false;
                ReturnToOriginalPosition();
            }
        }
    }
}
