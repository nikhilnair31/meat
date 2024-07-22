using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Throwable : PickableLimb
{
    private Image pickupIconImage;
    private bool isThrown = false;

    [Header("Throwable Properties")]
    [SerializeField] private ThrowableWeaponData weaponData;

    [Header("Impact Properties")]
    [SerializeField] private ImpactEffectData impactEffectData;

    public override void Interact() {
        Pickup();
    }
    public override void Pickup() {
        if (!isHeld) {
            isHeld = true;
            
            playerHand = playerInteract.playerInteractHolder;
            playerInteract.playerAnimator = animator;
            pickupIconImage = playerInteract.pickupIconImage;

            transform.SetParent(playerInteract.playerInteractHolder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = false;
            itemCollider.isTrigger = false;

            // animator.enabled = true;

            pickupIconImage.sprite = weaponData.pickupIcon;

            playerAttack.playerIsUnarmed = false;

            playerAnimations.ChangeAnimationState(weaponData.holdingAnimationName);

            ShowUI = false;
        }
        else {
            Debug.Log($"Item {gameObject.name} is already held");
        }
    }
    public override void Drop() {
        if (isHeld) {
            isHeld = false;

            transform.SetParent(null);
            
            itemRigidbody.isKinematic = false;
            itemRigidbody.useGravity = true;

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;

            // animator.enabled = false;

            playerAttack.playerIsUnarmed = true;

            playerInteract.pickupIconImage.sprite = null;

            playerAnimations.ChangeAnimationState();
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }

    protected override void Update() {
        base.Update();
        
        if (isHeld && Input.GetMouseButtonDown(0)) {
            Throw();
        }
    }

    private void Throw() {
        isHeld = false;
        isThrown = true;

        transform.SetParent(null);

        itemRigidbody.isKinematic = false;
        itemRigidbody.useGravity = true;

        itemCollider.enabled = true;
        itemCollider.isTrigger = false;

        playerAnimations.ChangeAnimationState(weaponData.throwingAnimationName);

        Vector3 throwPoint = Helper.CameraCenterTargetPoint();
        Vector3 throwVelocity = (throwPoint - transform.position).normalized * weaponData.throwForce;
        itemRigidbody.velocity = throwVelocity;

        playerInteract.pickupIconImage.sprite = null;

        playerAnimations.ChangeAnimationState();

        playerAttack.playerIsUnarmed = true;
            
        ShowUI = false;
    }

    private void OnCollisionEnter(Collision other) {
        if (!isHeld && isThrown) {
            if (other.collider.CompareTag("Limb")){
                Debug.Log($"Limb {other.transform.name} hit!");
                
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