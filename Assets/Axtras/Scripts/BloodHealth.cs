using UnityEngine;

public class BloodHealth : MonoBehaviour 
{
    [Header("Heal Properties")]
    [SerializeField] public int healAmount = 20;
    [SerializeField] public float healTime = 0f;

    // TODO: Update to make heal only when blood spurts not when it's not ground
    private void OnParticleCollision(GameObject other) {
        if (other.CompareTag("Player")) {
            if(other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth)) {
                Debug.Log("Player healing with blood");
                playerHealth.AddHealth(healAmount, healTime);
            }
        }
    }    
}