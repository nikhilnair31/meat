using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

[RequireComponent(typeof(CinemachineImpulseSource))]
[RequireComponent(typeof(AudioSource))]
public class PhysicsItem : MonoBehaviour 
{
    [Header("Random Settings")]
    [SerializeField] private bool randPos = false;
    [SerializeField] private float randPosAmount = 5f;
    [SerializeField] private bool randRot = false;
    [SerializeField] private float randRotAmount = 360f;

    [Header("Collision Settings")]
    [SerializeField] private List<string> allowedTags;
    [SerializeField] private ImpactEffectData impactEffectData;

    private void Start() {
        if (GetComponent<CinemachineImpulseSource>() == null) {
            gameObject.AddComponent<CinemachineImpulseSource>();
        }
        if (GetComponent<AudioSource>() == null) {
            gameObject.AddComponent<AudioSource>();
        }

        if(randPos) {
            transform.position += new Vector3(
                Random.Range(-randPosAmount, randPosAmount),
                Random.Range(-randPosAmount, randPosAmount),
                Random.Range(-randPosAmount, randPosAmount)
            );
        }
        if(randRot) {
            transform.rotation = Quaternion.Euler(
                Random.Range(-randRotAmount, randRotAmount), 
                Random.Range(-randRotAmount, randRotAmount), 
                Random.Range(-randRotAmount, randRotAmount)
            );
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (Helper.IsRelevantCollider(other.collider, allowedTags)) {
            PlayEffects();
        }
    }

    private void PlayEffects() {
        Helper.PlayOneShotWithRandPitch(
            GetComponent<AudioSource>(),
            impactEffectData.impactClip,
            impactEffectData.impactVolume,
            impactEffectData.randPitch
        );
        Helper.CameraImpulse(
            GetComponent<CinemachineImpulseSource>(),
            impactEffectData.hurtShakeMagnitude, 
            impactEffectData.hurtShakeDuration, 
            impactEffectData.hurtShakeMultiplier
        );
    }
}