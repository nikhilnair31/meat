using UnityEngine;

public class EnemyMelee : MonoBehaviour 
{
    private PlayerHealth playerHealth;
    private Test_Enemy enemyBehaviour;

    [Header("Weapon Properties")]
    [SerializeField] private MeleeWeaponData meleeWeaponData;

    private void Start() {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        enemyBehaviour = Helper.GetComponentInParentByTag<Test_Enemy>(transform, "Enemy");
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
                    meleeWeaponData.cameraShakeData.hurtShakeDuration, 
                    meleeWeaponData.cameraShakeData.hurtShakeMultiplier
                );
            }
        }
    }
}