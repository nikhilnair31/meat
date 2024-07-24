using UnityEngine;

public class EnemyMelee : MonoBehaviour 
{
    private PlayerHealth playerHealth;
    private EnemyBehaviour enemyBehaviour;

    [Header("Weapon Properties")]
    [SerializeField] private MeleeItemData meleeItemData;

    private void Start() {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        enemyBehaviour = Helper.GetComponentInParentByTag<EnemyBehaviour>(transform, "Enemy");
    }
    private void OnCollisionEnter(Collision other) {
        if (enemyBehaviour.IsAttacking) {
            if (other.collider.CompareTag("Player")) {
                playerHealth.DiffHealth(
                    false,
                    meleeItemData.damageAmount, 
                    meleeItemData.damageDuration
                );
                Helper.CameraShake(
                    meleeItemData.impactEffectData.hurtShakeMagnitude, 
                    meleeItemData.impactEffectData.hurtShakeDuration, 
                    meleeItemData.impactEffectData.hurtShakeMultiplier
                );
            }
        }
    }
    public void DisablePhysicsOnRagdoll() {
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<Rigidbody>());
    }
}