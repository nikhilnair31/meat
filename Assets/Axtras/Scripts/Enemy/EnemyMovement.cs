using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour 
{
    [SerializeField] private float lostSightDuration = 5f;
    [SerializeField] private float waitTimeAtLastSeen = 3f;

    private NavMeshAgent navMeshAgent;
    private Vector3 originalPosition;
    private Vector3 lastSeenPosition;
    private bool isChasing;
    private bool hasLostSight;
    private float lostSightTimer;
    private float waitTimer;

    public bool IsChasing => isChasing;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        originalPosition = transform.position;
        isChasing = false;
        hasLostSight = false;
        lostSightTimer = 0f;
        waitTimer = 0f;
    }

    public void StartChasing(Vector3 playerPosition)
    {
        isChasing = true;
        navMeshAgent.SetDestination(playerPosition);
    }

    public void StopChasing()
    {
        isChasing = false;
        ReturnToOriginalPosition();
    }

    public void UpdateLastSeenPosition(Vector3 playerPosition)
    {
        lastSeenPosition = playerPosition;
        lostSightTimer = 0f;
        hasLostSight = false;
        navMeshAgent.SetDestination(playerPosition);
    }

    public void HandleLostSight()
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

    private void GoToLastSeenPosition()
    {
        navMeshAgent.SetDestination(lastSeenPosition);
        waitTimer = 0f;
    }

    void Update()
    {
        if (hasLostSight)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer > waitTimeAtLastSeen)
            {
                ReturnToOriginalPosition();
            }
        }
    }

    private void ReturnToOriginalPosition()
    {
        navMeshAgent.SetDestination(originalPosition);
        hasLostSight = false;
        waitTimer = 0f;
    }
}