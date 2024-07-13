using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float detectionAngle = 45f;
    public float detectionDistance = 10f;
    public float chaseDistance = 15f;
    public float lostSightDuration = 5f;
    public float waitTimeAtLastSeen = 3f;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private Vector3 originalPosition;
    private Vector3 lastSeenPosition;
    private bool isChasing;
    private bool hasLostSight;
    private float lostSightTimer;
    private float waitTimer;

    // FIXME: Fix enemy logic
    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        originalPosition = transform.position;

        isChasing = false;
        hasLostSight = false;

        lostSightTimer = 0f;
        waitTimer = 0f;

        animator.SetBool("isStanding", true);
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (!isChasing && angleToPlayer < detectionAngle && distanceToPlayer < detectionDistance)
        {
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionDistance))
            {
                if (hit.transform == player)
                {
                    StartChasing();
                }
            }
        }

        if (isChasing)
        {
            if (distanceToPlayer < chaseDistance)
            {
                if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, chaseDistance))
                {
                    if (hit.transform == player)
                    {
                        hasLostSight = false;
                        lostSightTimer = 0f;
                        lastSeenPosition = player.position;
                        navMeshAgent.SetDestination(player.position);
                    }
                    else
                    {
                        if (!hasLostSight)
                        {
                            lostSightTimer += Time.deltaTime;
                            if (lostSightTimer > lostSightDuration)
                            {
                                hasLostSight = true;
                                GoToLastSeenPosition();
                            }
                        }
                    }
                }
            }
            else
            {
                StopChasing();
            }
        }

        if (hasLostSight && navMeshAgent.remainingDistance < 0.1f)
        {
            waitTimer += Time.deltaTime;
            animator.SetBool("isSearching", true);
            if (waitTimer > waitTimeAtLastSeen)
            {
                ReturnToOriginalPosition();
            }
        }
    }

    void StartChasing()
    {
        isChasing = true;

        navMeshAgent.SetDestination(player.position);

        animator.SetBool("isStanding", false);
        animator.SetBool("isChasing", true);
    }
    void StopChasing()
    {
        isChasing = false;

        animator.SetBool("isChasing", false);

        GoToLastSeenPosition();
    }

    void GoToLastSeenPosition()
    {
        waitTimer = 0f;
        
        navMeshAgent.SetDestination(lastSeenPosition);

        animator.SetBool("isChasing", true);
    }

    void ReturnToOriginalPosition()
    {
        waitTimer = 0f;
        hasLostSight = false;

        navMeshAgent.SetDestination(originalPosition);

        animator.SetBool("isChasing", false);
        animator.SetBool("isSearching", false);

        if (Vector3.Distance(transform.position, originalPosition) < 0.1f) {
            animator.SetBool("isStanding", true);
        }
    }
}
