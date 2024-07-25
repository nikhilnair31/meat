using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour 
{
    private EnemyAnimations enemyAnimations;

    private NavMeshAgent agent;
    // private Animator animator;

    private Transform player;

    private RaycastHit hit;

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
    [SerializeField] private Transform detectionRaycastSourceTransform;
    [SerializeField] private LayerMask detectionLayermask;
    [SerializeField] private float detectionDistanceRange = 15f;
    [SerializeField] private float detectionAngleRange = 90f;
    
    [Header("Search Settings")]
    [SerializeField] private float searchingDuration = 5f;
    private float searchingForTimer;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackDistanceRange = 2f;
    [SerializeField] private float attackLookAtPlayerSpeed = 5f;
    
    [Header("Reset Settings")]
    [SerializeField] private float resetAfterDuration = 10f;
    private float resetTimer;
    
    [Header("IsAtInitPos Settings")]
    [SerializeField] private string isAtInitPosAnim;
    [SerializeField] private string isChasingAnim;
    [SerializeField] private string isSearchingAnim;
    [SerializeField] private string isReturningAnim;
    [SerializeField] private string canAttackAnim;
    [SerializeField] private string isAttackingAnim;
    
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

            // animator.SetBool("isAtInitPos", isAtInitPos);
            enemyAnimations.ChangeAnimationState(isAtInitPosAnim, 1f);

            if (isAtInitPos) {
                IsSearching = false;
                IsReturning = false;
                IsChasing = false;

                if (agent != null && agent.enabled) {
                    agent.isStopped = true;
                    agent.speed = 0f;
                }
            }
        }
    }
    public bool IsChasing {
        get { return isChasing; }
        set { 
            isChasing = value; 

            // animator.SetBool("isChasing", isChasing);

            if (isChasing) {
                enemyAnimations.ChangeAnimationState(isChasingAnim, 1f);
                
                if (agent != null && agent.enabled) {
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(lastPlayerPosition);
                }
            }
        }
    }
    public bool IsSearching {
        get { return isSearching; }
        set { 
            isSearching = value; 

            // animator.SetBool("isSearching", isSearching);
            
            if (isSearching) {
                enemyAnimations.ChangeAnimationState(isSearchingAnim, 1f);
            }
        }
    }
    public bool IsReturning {
        get { return isReturning; }
        set { 
            isReturning = value; 

            // animator.SetBool("isReturning", isReturning);

            if (isReturning) {
                enemyAnimations.ChangeAnimationState(isReturningAnim, 1f);
                
                if (agent != null && agent.enabled) {
                    agent.isStopped = false;
                    agent.speed = walkSpeed;
                    agent.SetDestination(initialEnemyPosition);
                }
            }
        }
    }
    public bool IsAttacking {
        get { return isAttacking; }
        set { 
            isAttacking = value; 

            if (isAttacking) {
                FacePlayer();

                // animator.SetBool("isAttacking", true);
                // animator.SetTrigger("Attack");
                enemyAnimations.ChangeAnimationState(isAttackingAnim, 1f);

                if (agent != null && agent.enabled) {
                    agent.isStopped = true;
                    agent.speed = 0f;
                }
            }
            else {
                // animator.SetBool("isAttacking", false);
                // animator.ResetTrigger("Attack");
                enemyAnimations.ChangeAnimationState(canAttackAnim, 1f);

                if (agent != null && agent.enabled) {
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                }
            }
        }
    }

    private void Start() {
        player = GameObject.Find("Player").transform;

        enemyAnimations = GetComponent<EnemyAnimations>();
        agent = GetComponent<NavMeshAgent>();
        // animator = GetComponent<Animator>();

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
        if(Physics.Raycast(detectionRaycastSourceTransform.position, directionToPlayer, out hit, detectionDistanceRange, detectionLayermask)) {
            // Debug.Log($"Hit: {hit.transform.name}");
            inDetectionLOS = hit.transform == player;
        } else {
            inDetectionLOS = false;
        }
    }

    private void HandleStates() {
        if (IsAtInitPos) {
            if (inDetectionDistanceRange && inDetectionAngleRange && inDetectionLOS) {
                lastPlayerPosition = Helper.GetClosestPointOnNavMesh(player.position, 5f);
                IsAtInitPos = false;
                IsChasing = true;
            }
        } 
        else if (IsChasing) {
            if (playerDistance < attackDistanceRange) {
                IsAttacking = true;
            } 
            else {
                Invoke(nameof(StopAttacking), 1f);
            }

            if (!inDetectionDistanceRange) {
                if (lastPlayerPositionDistance < 2f) {
                    IsChasing = false;
                    IsSearching = true;
                }
                else {
                    IsSearching = false;
                }
            } 
            else {
                lastPlayerPosition = Helper.GetClosestPointOnNavMesh(player.position, 5f);
                if (agent != null && agent.enabled) {
                    agent.SetDestination(lastPlayerPosition);
                }
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

            if (initialEnemyPositionDistance < 3f) {
                IsReturning = false;
                IsAtInitPos = true;
            }
        }
    }

    private void StopAttacking() {
        IsAttacking = false;
    }

    private void Reseter() {
        if(agent.velocity.magnitude < 0.1f) {
            resetTimer += Time.deltaTime;
            if (resetTimer > resetAfterDuration) {
                IsReturning = true;
            }
        }
    }

    private void FacePlayer() {
        Vector3 direction = directionToPlayer.normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * attackLookAtPlayerSpeed);
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