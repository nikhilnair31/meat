using UnityEngine;

public class BloodHealth : MonoBehaviour 
{
    private ParticleSystem bloodParticleSystem;
    private ParticleSystem.CollisionModule collisionModule;

    [Header("Heal Properties")]
    [SerializeField] private int healAmount = 20;
    [SerializeField] private float healTime = 0f;
    [SerializeField] private LayerMask playerLayer;

    void Start() {
        bloodParticleSystem = GetComponent<ParticleSystem>();
        collisionModule = bloodParticleSystem.collision;
    }

    private void OnParticleCollision(GameObject other) {
        if (other.CompareTag("Player")) {
            if(other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth)) {
                Debug.Log("Player healing with blood");
                playerHealth.AddHealth(healAmount, healTime);
            }
        }
        if (other.CompareTag("Ground")) {
            collisionModule.collidesWith &= ~playerLayer;
        }
    }    
}