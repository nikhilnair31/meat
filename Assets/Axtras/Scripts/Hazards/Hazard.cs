using UnityEngine;
using Cinemachine;

public class Hazard : MonoBehaviour 
{
    [SerializeField] private float damageAmount = 1000f;
    [SerializeField] private CameraShakeData cameraShakeData;

    private void OnCollisionEnter(Collision other) {
        if (other.collider.CompareTag("Player")) {
            other.gameObject.GetComponent<PlayerHealth>().DiffHealth(damageAmount, 0.01f);
            GetComponent<AudioSource>().PlayOneShot(
                cameraShakeData.impactClip, 
                cameraShakeData.impactVolume
            );
            Helper.CameraImpulse(
                GetComponent<CinemachineImpulseSource>(),
                cameraShakeData.hurtShakeMagnitude, 
                cameraShakeData.hurtShakeDuration, 
                cameraShakeData.hurtShakeMultiplier
            );
        }
    }    
}