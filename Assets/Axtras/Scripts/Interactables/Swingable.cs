using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Swingable : PickableLimb
{
    public bool isAttacking = false;
    public bool isBlocking = false;

    [Header("Swingable Properties")]
    public SwingableWeaponData weaponData;

    [Header("Effects")]
    [SerializeField] private ImpactEffectData impactEffectData;

    public override void Interact() {
        Pickup();
    }
    public override void Pickup() {
        if (!isHeld) {
            isHeld = true;
            ShowUI = false;
            
            transform.SetParent(playerInteract.playerInteractHolder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;

            playerInteract.pickupIconImage.sprite = weaponData.pickupIcon;

            playerAttack.playerIsUnarmed = false;
        }                   
        else {
            Debug.Log($"Item {gameObject.name} is already held");
        }
    }
    public override void Drop() {
        if (isHeld) {
            isHeld = false;
            ShowUI = false;

            transform.SetParent(null);
            
            itemRigidbody.isKinematic = false;
            itemRigidbody.useGravity = true;

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;

            playerInteract.currentHeldItem = null;
            playerInteract.pickupIconImage.sprite = null;

            playerAttack.playerIsUnarmed = true;
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (isHeld) {
            if(isAttacking) {
                if (other.collider.CompareTag("Limb")) {
                    TransformCollector transformCollector = Helper.GetComponentInParentByTag<TransformCollector>(other.transform, "Enemy");
                    if (transformCollector != null) {
                        foreach (TransformData data in transformCollector.transformDataList) {
                            if(data.transformName.Contains(other.collider.name)) {
                                float scaledDamageAmount = weaponData.damageAmount * data.transformDamageMultiplier;

                                data.transformCurrentHealth -= scaledDamageAmount;
                            
                                EnemyHealth enemyHealth = Helper.GetComponentInParentByTag<EnemyHealth>(other.transform, "Enemy");
                                if (enemyHealth != null) {
                                    enemyHealth.DiffHealth(scaledDamageAmount, weaponData.damageDuration);
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

                // Decrease durability on collision
                ReduceDurabilityByCollision();

                Helper.PlayOneShotWithRandPitch(
                    GetComponent<AudioSource>(),
                    impactEffectData.impactClip,
                    impactEffectData.impactVolume,
                    impactEffectData.randPitch
                );
                Helper.CameraImpulse(
                    GetComponent<CinemachineImpulseSource>(),
                    impactEffectData.hurtShakeMagnitude, 
                    impactEffectData.hurtShakeDuration, 
                    impactEffectData.hurtShakeMultiplier
                );
            }
        }
    }
}
