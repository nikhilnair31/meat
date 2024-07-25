using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

[RequireComponent(typeof(CinemachineImpulseSource))]
[RequireComponent(typeof(AudioSource))]
public class PhysicsItem : MonoBehaviour 
{
    [SerializeField] private List<string> allowedTags;
    [SerializeField] private ImpactEffectData impactEffectData;

    private void Start() {
        if (GetComponent<CinemachineImpulseSource>() == null) {
            gameObject.AddComponent<CinemachineImpulseSource>();
        }
        if (GetComponent<AudioSource>() == null) {
            gameObject.AddComponent<AudioSource>();
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