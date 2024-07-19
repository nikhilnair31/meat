using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GroundSpikes : MonoBehaviour
{
    private bool triggerTimerToLaunch;
    private float standingOnTimer;
    [SerializeField] private List<string> allowedTags;
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
        if (Helper.IsRelevantCollider(other.collider, allowedTags)) {
            TriggerTimerToLaunch = true;
        }
    }

    // FIXME: Spike doesn't move back to its original position
    private void MoveSpikes() {
        float yValue = spikeTransform.localPosition.y;
        spikeTransform
            .DOLocalMoveY(spikeMoveY, spikeMoveInTime)
            .OnComplete(() => {
                spikeTransform
                .DOLocalMoveY(yValue, spikeMoveInTime)
                .OnComplete(() => {
                    TriggerTimerToLaunch = false;
                });
            });
    }
}