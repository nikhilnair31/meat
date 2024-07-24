using Cinemachine;
using UnityEngine;

public class Throwable : Pickable
{
    private bool isThrown = false;

    [Header("Throwable Properties")]
    public ThrowableItemData itemData;

    [Header("Impact Properties")]
    [SerializeField] private ImpactEffectData impactEffectData;

    public override void Pickup() {
        if (!isHeld) {
            isHeld = true;
            ShowUI = false;

            transform.SetParent(playerInteract.playerInteractHolder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = false;
            itemCollider.isTrigger = false;

            playerAnimations.ChangeAnimationState(itemData.holdingAnimnName);

            playerInteract.currentHeldItemType = PickableType.Throwable;
            playerInteract.pickupIconImage.sprite = itemData.pickupIcon;
        }
        else {
            Debug.Log($"Item {gameObject.name} is already held");
        }
    }
    public override void Drop(bool destroyItem) {
        if (isHeld) {
            isHeld = false;
            ShowUI = false;

            transform.SetParent(null);
            
            itemRigidbody.isKinematic = false;
            itemRigidbody.useGravity = true;

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;

            playerAnimations.ChangeAnimationState();

            playerInteract.currentHeldItem = null;
            playerInteract.currentHeldItemType = PickableType.None;
            playerInteract.pickupIconImage.sprite = playerAction.meleeWeaponData.weaponIcon;
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }

    public void Throw() {
        isHeld = false;
        isThrown = true;
        ShowUI = false;

        transform.SetParent(null);

        itemRigidbody.isKinematic = false;
        itemRigidbody.useGravity = true;

        itemCollider.enabled = true;
        itemCollider.isTrigger = false;

        Vector3 throwPoint = Helper.CameraCenterTargetPoint();
        Vector3 throwVelocity = (throwPoint - transform.position).normalized * itemData.throwForce;
        itemRigidbody.velocity = throwVelocity;

        playerInteract.currentHeldItem = null;
        playerInteract.currentHeldItemType = PickableType.None;
        playerInteract.pickupIconImage.sprite = itemData.pickupIcon;
    }

    private void OnCollisionEnter(Collision other) {
        if (!isHeld && isThrown) {
            if (other.collider.CompareTag("Limb")){
                Debug.Log($"Limb {other.transform.name} hit!");
                
                TransformCollector transformCollector = Helper.GetComponentInParentByTag<TransformCollector>(other.transform, "Enemy");
                if (transformCollector != null) {
                    foreach (TransformData data in transformCollector.transformDataList) {
                        if(data.transformName.Contains(other.collider.name)) {
                            float scaledDamageAmount = itemData.damageAmount * data.transformDamageMultiplier;

                            data.transformCurrentHealth -= scaledDamageAmount;
                            
                            EnemyHealth enemyHealth = Helper.GetComponentInParentByTag<EnemyHealth>(other.transform, "Enemy");
                            if (enemyHealth != null) {
                                enemyHealth.DiffHealth(scaledDamageAmount, itemData.damageDuration);
                            }
                        }
                    }
                }
                else {
                    Debug.LogError("TransformCollector not found on " + other.collider.name);
                }
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