using UnityEngine;
using DG.Tweening;

public class SawBlade : Hazard 
{
    [SerializeField] private Vector3 rotationAxis;
    [SerializeField] private float rotationSpeed = 1f;

    private void Start() {
        transform.DOLocalRotate(
            rotationAxis, 
            rotationSpeed, 
            RotateMode.FastBeyond360
        )
        .SetEase(Ease.Linear)
        .SetLoops(-1);
    }
}