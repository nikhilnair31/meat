using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour 
{
    [Header("Other")]
    public float lastSeenTime;

    [Header("Managers")]
    private MenuManager menuManagerScript;

    [Header("Self")]
    private EnemyMovement enemyMovementScript;

    [Header("Components")]
    public Animator animator;
    public NavMeshAgent navMeshAgent;

    [Header("Player")]
    public Transform playerTransform;
    
    [Header("Status")]
    public bool isPatroling = false;
    public bool isChasing = false;
    public bool isWaiting = true;
    public bool isDead = false;
    public bool isRagdoll = false;

    private void Awake() {
        if (navMeshAgent == null) {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        if (animator == null) {
            animator = GetComponent<Animator>();
        }

        if (playerTransform == null) {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        if (enemyMovementScript == null) {
            enemyMovementScript = GetComponent<EnemyMovement>();
        }

        if(menuManagerScript == null) {
            menuManagerScript = FindObjectOfType<MenuManager>();
        }
    }

    public bool IsPatroling {
        get { 
            return isPatroling; 
        }
        set {
            if (isPatroling != value) {
                isPatroling = value;
                animator.SetBool("isPatroling", isPatroling);
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
                animator.SetBool("isChasing", isChasing);
            }
        }
    }
    public bool IsWaiting {
        get { 
            return isWaiting; 
        }
        set {
            if (isWaiting != value) {
                isWaiting = value;
                animator.SetBool("isWaiting", isWaiting);
            }
        }
    }
    public bool IsDead {
        get { 
            return isDead; 
        }
        set {
            if (isDead != value) {
                isDead = value;
                if (isDead) {
                    // 
                }
                else {
                    // 
                }
            }
        }
    }
}