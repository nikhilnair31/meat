using UnityEngine;

public class Fireplace : MonoBehaviour 
{
    private PlayerHealth playerHealth;
    private float nextHurtTime;
    private bool isOn = false;

    [Header("Main")]
    [SerializeField] private ParticleSystem flameParticle;

    [Header("Damage Properties")]
    [SerializeField] private int hurtAmountOnEnter = 20;
    [SerializeField] private int hurtAmountOnStay = 20;
    [SerializeField] private float hurtStayRate = 1f;

    [Header("Camera Shake Effects")]
    [SerializeField] private float hurtShakeMagnitude = 4.0f;
    [SerializeField] private float hurtShakeDuration = 0.4f;
    [SerializeField] private float hurtShakeMultiplier = 1.2f;

    private void Start() {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    public void StartStopFireplace() {
        if (isOn = !isOn) flameParticle.Play(); else flameParticle.Stop();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerHealth.DiffHealth(hurtAmountOnEnter);
            Helper.CameraShake(hurtShakeMagnitude * hurtShakeMultiplier, hurtShakeDuration * hurtShakeMultiplier);
        }
    }    
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") && Time.time >= nextHurtTime) {
            playerHealth.DiffHealth(hurtAmountOnStay);
            Helper.CameraShake(hurtShakeMagnitude, hurtShakeDuration);
            
            nextHurtTime = Time.time + hurtStayRate;
        }
    }
}
