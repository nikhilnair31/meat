using System.Collections;
using UnityEngine;

public class EnemyBehaviour : Enemy 
{
    [Header("Detection Settings")]
    [SerializeField] private LayerMask detectionLayer;
    [SerializeField] private float detectionAngle = 45f;
    [SerializeField] private float detectionDistance = 10f;

    [Header("Walk Settings")]
    [SerializeField] private float walkSpeed = 4f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float chaseDistance = 15f;
    
    [Header("Search Settings")]
    [SerializeField] private float searchingForDuration = 5f;
    [SerializeField] private float searchingForTimer;

    [Header("Attack Settings")]
    [SerializeField] private EnemyMelee enemyMelee;
    [SerializeField] private float attackDistance = 3f;
    [SerializeField] private float attackTime = 1f;
    
    [Header("Reset Settings")]
    [SerializeField] private float resetAfterDuration = 10f;
    private float resetTimer;
    
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
    public bool IsWalking {
        get { 
            return isWalking; 
        }
        set {
            if (isWalking != value) {
                isWalking = value;
                navMeshAgent.speed = value ? walkSpeed : 0f;
                animator.SetBool("isWalking", value);
            }
        }
    }
    public bool IsChasing {
        get { 
            return isChasing; 
        }
        set {
            if (isChasing != value) {
                isChasing = value;
                navMeshAgent.speed = value ? chaseSpeed : 0f;
                animator.SetBool("isChasing", value);
            }
            IsIdle = false;
        }
    }
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
    // TODO: Update to make enemy move back when too close for an attack
    public bool IsAttacking {
        get { 
            return isAttacking; 
        }
        set {
            if (isAttacking != value) {
                isAttacking = value;
                if(isAttacking) {
                    navMeshAgent.isStopped = true;
                    navMeshAgent.speed = 0f;
                    animator.SetTrigger("Attack");
                }
                else {
                    navMeshAgent.isStopped = false;
                    navMeshAgent.speed = walkSpeed;
                    animator.ResetTrigger("Attack");
                }
                animator.SetBool("isAttacking", value);
            }
        }
    }

    protected override void Start() {
        base.Start();
        
        searchingForTimer = 0f;
        resetTimer = 0f;

        IsIdle = true;
        IsChasing = false;
        IsAttacking = false;
    }

    protected override void Update() {
        base.Update();
        
        Detection();
        Idle();
        Searching();
        Attack();
        Reseter();
    }

    private void Detection() {
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
            // If player is within chase distance but outside of attack distance
            if (distanceToPlayer < chaseDistance && distanceToPlayer > attackDistance) {
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
            else if (distanceToPlayer > chaseDistance) {
                ReturnToOriginalPosition();
            }
            // If player is within attack distance then attack
            else if (distanceToPlayer < attackDistance) {
                StartCoroutine(AttackOverTime(attackTime));
            }
            // If player is out of attack distance then return to original position
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
    private void Reseter() {
        if(navMeshAgent.velocity.magnitude < 0.1f) {
            resetTimer += Time.deltaTime;
            if (resetTimer > resetAfterDuration) {
                IsSearching = false;
                ReturnToOriginalPosition();
            }
        }
    }

    private void Idle() {
        if(!IsIdle) {
            if (distanceToOriginalPosition < 1.1f) {
                IsIdle = true;
            }
        }
    }

    private void Searching() {
        // If not currently searching for player
        if(!IsSearching) {
            // If not currently chasing the player
            if (!IsChasing) {
                if (distanceToLastSeenPosition < 1.1f) {
                    IsSearching = true;
                }
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

    private void Attack() {
        if (distanceToPlayer < attackDistance) {
            StartCoroutine(AttackOverTime(attackTime));
        }
    }
    private IEnumerator AttackOverTime(float duration) {
        IsAttacking = true;
        
        yield return new WaitForSeconds(duration);
        
        IsAttacking = false;
    }
    public void EnableKickColl() {
        enemyMelee.kickCollider.enabled = true;
    }
    public void DisableKickWeapon() {
        Destroy(enemyMelee.kickRGB);
        Destroy(enemyMelee.kickCollider);

        navMeshAgent.enabled = false;
        this.enabled = false;
    }    
}