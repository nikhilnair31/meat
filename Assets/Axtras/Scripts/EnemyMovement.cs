using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : Enemy 
{
    [Header("Patrol Related")]
    [SerializeField] private Transform patrolPointsParent;
    [SerializeField] private bool selectPatrolPointsAtRandom = true;
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float waitTimeAtPatrolPoints = 2f;
    private List<Transform> patrolPoints = new();
    private int currentPatrolIndex;
    
    [Header("Chase Related")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float loseSightTime = 3f;

    [Header("Player Checking Related")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float visionAngle = 45f;
    [SerializeField] private float visionRange = 10f;
    private Vector3 directionToPlayer;  

    private Vector3 lastKnownPosition;
    private bool isInvestigatingLastKnownPosition;

    private void Start() {
        if(selectPatrolPointsAtRandom) {
            SelectRandomPatrolPoints();
        }

        currentPatrolIndex = 0;
        navMeshAgent.speed = patrolSpeed;
        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        navMeshAgent.isStopped = true;
    }

    private void SelectRandomPatrolPoints() {
        foreach (Transform item in patrolPointsParent) {
            patrolPoints.Add(item);
        }
        for (int i = patrolPoints.Count - 1; i > 0; i--) {
            int randomIndex = Random.Range(0, i + 1);
            Transform temp = patrolPoints[i];
            patrolPoints[i] = patrolPoints[randomIndex];
            patrolPoints[randomIndex] = temp;
            // (patrolPoints[randomIndex], patrolPoints[i]) = (patrolPoints[i], patrolPoints[randomIndex]);
        }
    }

    private void Update() {
        // Only Chase/Patrol/Hunt when not stunned
        if(!isRagdoll) {
            if (IsChasing) {
                ChasePlayer();
            } 
            else if (isInvestigatingLastKnownPosition) {
                InvestigateLastKnownPosition();
            } 
            else {
                if (!IsWaiting) {
                    PatrolAbout();
                }
            }

            HuntPlayer();
        }
    }

    private void ChasePlayer() {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(playerTransform.position);

        if (Time.time - lastSeenTime > loseSightTime) {
            isInvestigatingLastKnownPosition = true;
            IsChasing = false;
            navMeshAgent.SetDestination(lastKnownPosition);
        }
    }

    private void InvestigateLastKnownPosition() {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = patrolSpeed;

        if (navMeshAgent.remainingDistance < 1f) {
            StartCoroutine(WaitAtLastKnownPosition());
        }
    }
    private IEnumerator WaitAtLastKnownPosition() {
        IsWaiting = true;
        navMeshAgent.isStopped = true;
        
        yield return new WaitForSeconds(waitTimeAtPatrolPoints * 2f);
        
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        isInvestigatingLastKnownPosition = false;
        IsWaiting = false;
    }

    private void PatrolAbout() {
        Debug.Log("PatrolAbout");
        if (navMeshAgent.remainingDistance < 1f) {
            Debug.Log("navMeshAgent.remainingDistance < 1f");
            StartCoroutine(WaitAtPatrolPoint());
        }
        else {
            IsPatroling = true;
        }
    }
    private IEnumerator WaitAtPatrolPoint() {
        IsWaiting = true;
        navMeshAgent.isStopped = true;
        
        yield return new WaitForSeconds(waitTimeAtPatrolPoints);
        
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        navMeshAgent.isStopped = false;
        IsWaiting = false;
    }

    private void HuntPlayer() {
        directionToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if(!IsChasing) {
            if (distanceToPlayer < visionRange) {
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
                if (angleToPlayer < visionAngle / 2f) {
                    CheckPlayerInSight();
                }
            }
        }
        else {
            CheckPlayerInSight();
        }
    }
    private void CheckPlayerInSight() {
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, playerLayer)) {
            if (hit.transform == playerTransform) {
                IsChasing = true;
                isInvestigatingLastKnownPosition = false;
                lastKnownPosition = playerTransform.position;
                lastSeenTime = Time.time;
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lastKnownPosition, 1f);
    }   
}