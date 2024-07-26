using UnityEngine;

public class BloodHealth : Blood 
{
    [Header("Heal Properties")]
    [SerializeField] private float healAmount = 20;
    [SerializeField] private float healTime = 0f;
    
    protected override void OnParticleCollision(GameObject other) {
        base.OnParticleCollision(other);

        int numCollisionEvents = bloodParticleSystem.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++) {
            if (other.CompareTag("Player")) {
                if (other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth)) {
                    Debug.Log("Player healing with blood");
                    playerHealth.AddHealth(healAmount, healTime);
                }
            }
        }
    }
}