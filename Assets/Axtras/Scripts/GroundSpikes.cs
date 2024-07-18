using UnityEngine;
using DG.Tweening;

public class GroundSpikes : MonoBehaviour
{
    private bool triggerTimerToLaunch;
    private float standingOnTimer;
    [SerializeField] private Transform spikeTransform;
    [SerializeField] private float spikeMoveY = 0.5f;
    [SerializeField] private float spikeMoveInTime = 0.5f;
    [SerializeField] private float spikeInTime = 5f;
    [SerializeField] private float damageAmount = 1000f;

    public bool TriggerTimerToLaunch {
        get { return triggerTimerToLaunch; }
        set {
            if (triggerTimerToLaunch != value) {
                triggerTimerToLaunch = value;
                standingOnTimer = 0f;
            }
        }
    }

    private void Start() {
        TriggerTimerToLaunch = false;
    }

    private void Update() {
        if (TriggerTimerToLaunch) {
            standingOnTimer += Time.deltaTime;
            if (standingOnTimer >= spikeInTime) {
                MoveSpikes();
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (IsRelevantCollider(other.collider)) {
            TriggerTimerToLaunch = true;
        }
    }

    private void MoveSpikes() {
        float yValue = spikeTransform.localPosition.y;
        spikeTransform
            .DOLocalMoveY(spikeMoveY, spikeMoveInTime)
            .OnComplete(() => {
                spikeTransform
                .DOLocalMoveY(-spikeMoveY, spikeMoveInTime)
                .OnComplete(() => {
                    TriggerTimerToLaunch = false;
                });
            });
    }

    private bool IsRelevantCollider(Collider collider) {
        return collider.CompareTag("Player") || collider.CompareTag("Enemy") || collider.CompareTag("Physics");
    }
}