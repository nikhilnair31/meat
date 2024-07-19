using UnityEngine;
using UnityEngine.AI;

public class Test_Enemy : MonoBehaviour 
{
    private RaycastHit hit;

    private Transform player;

    private NavMeshAgent agent;
    private Animator animator;

    private Vector3 initialEnemyPosition;
    private Vector3 lastPlayerPosition;
    private Vector3 directionToPlayer;

    private float playerDistance;
    private float playerAngle;
    private float initialEnemyPositionDistance;
    private float lastPlayerPositionDistance;

    [Header("Speed Settings")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float chaseSpeed = 6f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionDistanceRange = 15f;
    [SerializeField] private float detectionAngleRange = 90f;
    
    [Header("Search Settings")]
    [SerializeField] private float searchingDuration = 5f;
    private float searchingForTimer;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackDistanceRange = 2f;
    
    [Header("Reset Settings")]
    [SerializeField] private float resetAfterDuration = 10f;
    private float resetTimer;

    private bool inDetectionDistanceRange;
    private bool inDetectionAngleRange;
    private bool inDetectionLOS;
    private bool isAtInitPos;
    private bool isChasing;
    private bool isAttacking;
    private bool isSearching;
    private bool isReturning;

    public bool IsAtInitPos {
        get { return isAtInitPos; }
        set { 
            isAtInitPos = value; 

            animator.SetBool("isAtInitPos", isAtInitPos);

            if (isAtInitPos) {
                IsSearching = false;
                IsReturning = false;
                IsChasing = false;

                agent.isStopped = true;
                agent.speed = 0f;
            }
        }
    }
    public bool IsChasing {
        get { return isChasing; }
        set { 
            isChasing = value; 

            animator.SetBool("isChasing", isChasing);

            if (isChasing) {
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                agent.SetDestination(lastPlayerPosition);
            }
        }
    }
    public bool IsSearching {
        get { return isSearching; }
        set { 
            isSearching = value; 
            animator.SetBool("isSearching", isSearching);
        }
    }
    public bool IsReturning {
        get { return isReturning; }
        set { 
            isReturning = value; 

            animator.SetBool("isReturning", isReturning);

            if (isReturning) {
                agent.isStopped = false;
                agent.speed = walkSpeed;
                agent.SetDestination(initialEnemyPosition);
            }
        }
    }
    public bool IsAttacking {
        get { return isAttacking; }
        set { 
            isAttacking = value; 

            if (isAttacking) {
                animator.SetBool("isAttacking", true);
                animator.SetTrigger("Attack");

                agent.isStopped = true;
                agent.speed = 0f;
            }
            else {
                animator.SetBool("isAttacking", false);
                animator.ResetTrigger("Attack");

                agent.isStopped = false;
                agent.speed = chaseSpeed;
            }
        }
    }

    private void Start() {
        player = GameObject.Find("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        initialEnemyPosition = transform.position;
        lastPlayerPosition = Vector3.zero;

        agent.speed = walkSpeed;

        IsAtInitPos = true;
    } 

    private void Update() {
        Calc();
        Detect();
        HandleStates();
        // Reseter();
    }

    private void Calc() {
        directionToPlayer = player.position - transform.position;
        playerDistance = Vector3.Distance(transform.position, player.position);
        playerAngle = Vector3.Angle(transform.forward, directionToPlayer);
        initialEnemyPositionDistance = Vector3.Distance(transform.position, initialEnemyPosition);
        lastPlayerPositionDistance = Vector3.Distance(transform.position, lastPlayerPosition);
    }

    private void Detect() {
        DetectDistance();
        DetectAngle();
        DetectLOS();
    }
    private void DetectDistance() {
        inDetectionDistanceRange = playerDistance < detectionDistanceRange;
    }
    private void DetectAngle() {
        inDetectionAngleRange = playerAngle < detectionAngleRange;
    }
    private void DetectLOS() {
        if(Physics.Raycast(transform.position, directionToPlayer, out hit, detectionDistanceRange)) {
            inDetectionLOS = hit.transform == player;
        } else {
            inDetectionLOS = false;
        }
    }

    private void HandleStates() {
        if (IsAtInitPos) {
            if (inDetectionDistanceRange && inDetectionAngleRange && inDetectionLOS) {
                lastPlayerPosition = player.position;
                IsAtInitPos = false;
                IsChasing = true;
            }
        } 
        else if (IsChasing) {
            if (playerDistance < attackDistanceRange) {
                IsAttacking = true;
            } 
            else {
                IsAttacking = false;
            }

            if (!inDetectionDistanceRange || !inDetectionLOS) {
                if (lastPlayerPositionDistance < 2f) {
                    IsChasing = false;
                    IsSearching = true;
                }
                else {
                    IsSearching = false;
                }
            } 
            else {
                lastPlayerPosition = player.position;
                agent.SetDestination(lastPlayerPosition);
            }
        } 
        else if (IsSearching) {
            if (inDetectionDistanceRange && inDetectionAngleRange && inDetectionLOS) {
                IsSearching = false;
                IsChasing = true;
            } 
            else {
                searchingForTimer += Time.deltaTime;
                if (searchingForTimer > searchingDuration) {
                    IsSearching = false;
                    IsReturning = true;
                    searchingForTimer = 0;
                } 
                else {
                    agent.SetDestination(lastPlayerPosition);
                }
            }
        } 
        else if (IsReturning) {
            if (inDetectionDistanceRange && inDetectionAngleRange && inDetectionLOS) {
                IsReturning = false;
                IsChasing = true;
            } 

            if (initialEnemyPositionDistance < 2f) {
                IsReturning = false;
                IsAtInitPos = true;
            }
        }
    }

    private void Reseter() {
        if(agent.velocity.magnitude < 0.1f) {
            resetTimer += Time.deltaTime;
            if (resetTimer > resetAfterDuration) {
                IsReturning = true;
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, detectionDistanceRange);
        Gizmos.color = Color.white;
        
        Gizmos.DrawSphere(initialEnemyPosition, 0.5f);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(lastPlayerPosition, 0.5f);
        Gizmos.color = Color.red;
    }
}