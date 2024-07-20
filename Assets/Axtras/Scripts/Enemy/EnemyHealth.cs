using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour 
{
    [Header("Health Properties")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    private void Start() {
        currentHealth = maxHealth;
    }
    
    public void DiffHealth(float amount, float duration) {
        StartCoroutine(DiffHealthOverTime(amount, duration));
    }
    private IEnumerator DiffHealthOverTime(float amount, float duration) {
        float elapsedTime = 0f;
        float initialHealth = currentHealth;
        float targetHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            currentHealth = Mathf.RoundToInt(Mathf.Lerp(initialHealth, targetHealth, elapsedTime / duration));

            if (currentHealth <= 0) {
                currentHealth = 0;
                EnemyDead();
                break;
            }

            yield return null;
        }

        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
    
    private void EnemyDead() {
        if (TryGetComponent<EnemyRagdoll>(out var enemyRagdoll)) {
            enemyRagdoll.EnableRagdoll();
        }     
        if (TryGetComponent<EnemyBehaviour>(out var enemyBehaviour)) {
            enemyBehaviour.enabled = false;
        }
        if (TryGetComponent<EnemyHealth>(out var enemyHealth)) {
            enemyHealth.enabled = false;
        }
    }
}