using UnityEngine;
using UnityEngine.AI;

public class Test_Enemy : MonoBehaviour 
{
    private RaycastHit hit;

    [SerializeField] private Transform player;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [SerializeField] private Vector3 initialEnemyPosition;
    [SerializeField] private Vector3 lastPlayerPosition;
    [SerializeField] private Vector3 directionToPlayer;

    [SerializeField] private float playerDistance;
    [SerializeField] private float playerAngle;
    [SerializeField] private float initialEnemyPositionDistance;
    [SerializeField] private float lastPlayerPositionDistance;
    [SerializeField] private float lostSightForTime;

    private bool inDetectionDistanceRange;
    private bool inDetectionAngleRange;
    private bool inDetectionLOS;
    private bool isAtInitPos;
    private bool isReturning;
    private bool isChasing;
    private bool isSearching;
    private bool isAttacking;
    private bool inAttackDistanceRange;

    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float detectionDistanceRange = 15f;
    [SerializeField] private float detectionAngleRange = 90f;
    [SerializeField] private float lostSightForTimeMax = 5f;
    [SerializeField] private float attackDistanceRange = 2f;

    public bool IsAtInitPos {
        get { return isAtInitPos; }
        set { 
            isAtInitPos = value; 
            if (isAtInitPos) {
                IsSearching = false;
                IsReturning = false;
                IsChasing = false;

                animator.SetBool("isAtInitPos", true);

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
                agent.SetDestination(player.position);
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

        // Chasing();
        // Returning();
        // Wait();
        // Attack();
        // Search();
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
                IsChasing = true;
                IsAtInitPos = false;
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
                // lostSightForTime += Time.deltaTime;
                // if (lostSightForTime > lostSightForTimeMax) {
                //     IsChasing = false;
                //     IsSearching = true;
                //     lostSightForTime = 0;
                // }
                IsChasing = false;
                IsSearching = true;
            } 
            else {
                lostSightForTime = 0;
                agent.SetDestination(player.position);
                lastPlayerPosition = player.position;
            }
        } 
        else if (IsSearching) {
            if (inDetectionDistanceRange && inDetectionLOS) {
                IsSearching = false;
                IsChasing = true;
            } 
            else {
                lostSightForTime += Time.deltaTime;
                if (lostSightForTime > lostSightForTimeMax) {
                    IsSearching = false;
                    IsReturning = true;
                    lostSightForTime = 0;
                } 
                else {
                    agent.SetDestination(lastPlayerPosition);
                }
            }
        } 
        else if (IsReturning) {
            if (initialEnemyPositionDistance < 2f) {
                IsReturning = false;
                IsAtInitPos = true;
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