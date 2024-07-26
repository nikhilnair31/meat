using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GroundSpikes : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool triggerTimerToLaunch;
    [SerializeField] private List<string> allowedTags;
    [SerializeField] private Transform spikeTransform;
    [SerializeField] private float spikeMoveY = 0.5f;
    [SerializeField] private float spikeMoveInTime = 0.5f;
    [SerializeField] private float spikeInTime = 5f;
    [SerializeField] private float damageAmount = 1000f;

    private void Start() {
        originalPosition = spikeTransform.position;
        triggerTimerToLaunch = false;
    }

    private void OnCollisionEnter(Collision other) {
        if (Helper.IsRelevantCollider(other.collider, allowedTags)) {
            MoveSpikes();
        }
    }

    private void MoveSpikes() {
        triggerTimerToLaunch = true;

        Sequence sequence = DOTween.Sequence();
        sequence
        .AppendInterval(spikeInTime)
        .Append(spikeTransform.DOMoveY(spikeTransform.position.y + spikeMoveY, spikeMoveInTime))
        .AppendInterval(spikeInTime)
        .Append(spikeTransform.DOMove(originalPosition, spikeMoveInTime));
    }
}