using UnityEngine;

public class EnemyMelee : MonoBehaviour 
{
    private PlayerHealth playerHealth;
    private EnemyBehaviour enemyBehaviour;

    [Header("Weapon Properties")]
    [SerializeField] private MeleeWeaponData meleeWeaponData;

    private void Start() {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        enemyBehaviour = Helper.GetComponentInParentByTag<EnemyBehaviour>(transform, "Enemy");
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
    public void DisableOnRagdoll() {
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<Rigidbody>());
    }
}