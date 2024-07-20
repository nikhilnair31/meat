using UnityEngine;
using Cinemachine;

public class Hazard : MonoBehaviour 
{
    [SerializeField] private bool hurtOnTouch = true;
    [SerializeField] private float damageAmount = 1000f;
    [SerializeField] private ImpactEffectData impactEffectData;

    private void OnCollisionEnter(Collision other) {
        if (hurtOnTouch) {
            if (other.collider.CompareTag("Player")) {
                other.gameObject.GetComponent<PlayerHealth>().DiffHealth(damageAmount, 0.01f);
            }
        }
        PlayEffects();
    }    

    private void PlayEffects() {
        GetComponent<AudioSource>().PlayOneShot(
            impactEffectData.impactClip, 
            impactEffectData.impactVolume
        );
        Helper.CameraImpulse(
            GetComponent<CinemachineImpulseSource>(),
            impactEffectData.hurtShakeMagnitude, 
            impactEffectData.hurtShakeDuration, 
            impactEffectData.hurtShakeMultiplier
        );
    }
}