using UnityEngine;

public class Fireplace : Interactable 
{
    private PlayerHealth playerHealth;

    [Header("Main")]
    [SerializeField] private bool isOff = false;
    [SerializeField] private ParticleSystem flameParticle;

    [Header("Damage Properties")]
    [SerializeField] private int hurtAmountOnEnter = 20;
    [SerializeField] private int hurtAmountOnStay = 20;
    [SerializeField] private float hurtStayRate = 1f;
    private float nextHurtTime;

    [Header("Camera Shake Effects")]
    [SerializeField] private float hurtShakeMagnitude = 4.0f;
    [SerializeField] private float hurtShakeDuration = 0.4f;
    [SerializeField] private float hurtShakeMultiplier = 1.2f;

    private void Start() {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        StartStopFireplace();
    }

    public override void Interact() {
        StartStopFireplace();
    }
    public override void Pickup() {
    }
    public override void Drop() {
    }

    public void StartStopFireplace() {
        if (isOff = !isOff) flameParticle.Play(); else flameParticle.Stop();
    }

    private void OnTriggerEnter(Collider other) {
        if(isOff) {
            if (other.CompareTag("Player")) {
                playerHealth.DiffHealth(hurtAmountOnEnter, 0.01f);
                Helper.CameraShake(hurtShakeMagnitude * hurtShakeMultiplier, hurtShakeDuration * hurtShakeMultiplier);
            }
        }
    }    
    private void OnTriggerStay(Collider other) {
        if(isOff) {
            if (other.CompareTag("Player") && Time.time >= nextHurtTime) {
                playerHealth.DiffHealth(hurtAmountOnStay, 0.01f);
                Helper.CameraShake(hurtShakeMagnitude, hurtShakeDuration);
                
                nextHurtTime = Time.time + hurtStayRate;
            }
        }
    }
}
