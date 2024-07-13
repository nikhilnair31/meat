using UnityEngine;

public class BloodHealth : MonoBehaviour 
{
    [Header("Heal Properties")]
    [SerializeField] public int healAmount = 20;
    [SerializeField] public float healTime = 0f;

    private void OnParticleCollision(GameObject other) {
        if (other.CompareTag("Player")) {
            if(other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth)) {
                Debug.Log("Player healing with blood");
                playerHealth.AddHealth(healAmount, healTime);
            }
        }
    }    
}