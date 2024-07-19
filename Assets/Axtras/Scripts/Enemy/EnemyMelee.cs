using UnityEngine;

public class EnemyMelee : MonoBehaviour 
{
    private PlayerHealth playerHealth;
    private Test_Enemy enemyBehaviour;

    [Header("Weapon Properties")]
    [SerializeField] private MeleeWeaponData meleeWeaponData;

    // [Header("Collision Properties")]
    // public Collider kickCollider;
    // public Rigidbody kickRGB;

    private void Start() {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        enemyBehaviour = Helper.GetComponentInParentByTag<Test_Enemy>(transform, "Enemy");

        // kickCollider = GetComponent<Collider>();
        // kickRGB = GetComponent<Rigidbody>();

        // kickCollider.enabled = false;
    }
    private void OnCollisionEnter(Collision other) {
        if (enemyBehaviour.IsAttacking) {
            if (other.collider.CompareTag("Player")) {
                playerHealth.DiffHealth(
                    meleeWeaponData.damageAmount, 
                    meleeWeaponData.damageDuration
                );
                Helper.CameraShake(
                    meleeWeaponData.cameraShakeData.hurtShakeMagnitude, 
                    meleeWeaponData.cameraShakeData.hurtShakeDuration
                );
                // kickCollider.enabled = false;
            }
        }
    }
}