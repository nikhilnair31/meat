using Cinemachine;
using UnityEngine;

public class PlayerMelee : MonoBehaviour 
{
    private PlayerAttack playerAttack;

    [Header("Weapon Properties")]
    [SerializeField] private MeleeWeaponData meleeWeaponData;

    private void Start() {
        playerAttack = Helper.GetComponentInParentByTag<PlayerAttack>(transform, "Player");
    }
    private void OnCollisionEnter(Collision other) {
        if (playerAttack.isAttacking) {
            if (other.collider.CompareTag("Limb")) {
                TransformCollector transformCollector = Helper.GetComponentInParentByTag<TransformCollector>(other.transform, "Enemy");
                if (transformCollector != null) {
                    foreach (TransformData data in transformCollector.transformDataList) {
                        if(data.transformName.Contains(other.collider.name)) {
                            float scaledDamageAmount = meleeWeaponData.damageAmount * data.transformDamageMultiplier;

                            data.transformCurrentHealth -= scaledDamageAmount;
                        
                            EnemyHealth enemyHealth = Helper.GetComponentInParentByTag<EnemyHealth>(other.transform, "Enemy");
                            if (enemyHealth != null) {
                                enemyHealth.DiffHealth(scaledDamageAmount, meleeWeaponData.damageDuration);
                            }
                        }
                    }
                }
                else {
                    Debug.LogError("TransformCollector not found on " + other.collider.name);
                }
            }
            else {
                // Deal damage to the player
                Debug.Log("Player hit something else!");
            }

            Helper.PlayOneShotWithRandPitch(
                GetComponent<AudioSource>(),
                meleeWeaponData.impactEffectData.impactClip,
                meleeWeaponData.impactEffectData.impactVolume,
                meleeWeaponData.impactEffectData.randPitch
            );
            Helper.CameraShake(
                meleeWeaponData.impactEffectData.hurtShakeMagnitude, 
                meleeWeaponData.impactEffectData.hurtShakeDuration, 
                meleeWeaponData.impactEffectData.hurtShakeMultiplier
            );
        }
    } 
}