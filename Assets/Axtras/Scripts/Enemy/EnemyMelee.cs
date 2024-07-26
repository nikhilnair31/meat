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

                foreach (ImpactEffectData impactEffectData in meleeItemData.impactEffectDatas) {
                    ShowEffects(other, impactEffectData);
                }
            }
        }
    }

    private void ShowEffects(Collision hit, ImpactEffectData impactEffectData) {
        if(hit.transform.gameObject.layer == impactEffectData.layer.value) {
            Helper.CameraShake(
                impactEffectData.hurtShakeMagnitude,
                impactEffectData.hurtShakeDuration,
                impactEffectData.hurtShakeMultiplier
            );

            Helper.PlayOneShotWithRandPitch(
                GetComponent<AudioSource>(),
                impactEffectData.impactClip,
                impactEffectData.impactVolume,
                impactEffectData.randPitch
            );

            GameObject impactParticle = Instantiate(
                impactEffectData.impactParticlePrefab, 
                hit.transform.position,
                Quaternion.identity
            );
            if (impactEffectData.destroyAfterInstantiation) {
                Destroy(impactParticle, impactEffectData.destroyDelay);
            }
        }
    }

    public void DisablePhysicsOnRagdoll() {
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<Rigidbody>());
    }
}