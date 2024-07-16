using UnityEngine;

public class EnemyKick : MonoBehaviour 
{
    private PlayerHealth playerHealth;
    private Enemy enemy;

    [Header("Collision Properties")]
    public Collider kickCollider;

    [Header("Damage Properties")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageDuration = 0.01f;

    [Header("Camera Shake Effects")]
    [SerializeField] private float hurtShakeMagnitude = 4.0f;
    [SerializeField] private float hurtShakeDuration = 0.4f;
    [SerializeField] private float hurtShakeMultiplier = 1.2f;

    private void Start() {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        enemy = Helper.GetComponentInParentByTag<Enemy>(transform, "Enemy");

        kickCollider = GetComponent<Collider>();

        kickCollider.enabled = false;
    }
    private void OnCollisionEnter(Collision other) {
        if (enemy.IsAttacking) {
            if (other.collider.CompareTag("Player")) {
                ApplyDamage(damageAmount, damageDuration);
                kickCollider.enabled = false;
            }
        }
    }

    private void ApplyDamage(int damage, float duration) {
        playerHealth.DiffHealth(damage, 0.01f);
        Helper.CameraShake(hurtShakeMagnitude * hurtShakeMultiplier, hurtShakeDuration * hurtShakeMultiplier);
    }
}