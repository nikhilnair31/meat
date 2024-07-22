using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Swingable : PickableLimb
{
    private Image pickupIconImage;

    [Header("Main")]
    public bool isAttacking = false;
    public bool isBlocking = false;

    [Header("Swingable Properties")]
    [SerializeField] private SwingableWeaponData weaponData;

    [Header("Effects")]
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

            transform.SetParent(playerHand);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            itemRigidbody.isKinematic = false;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;

            // animator.enabled = true;

            pickupIconImage.sprite = weaponData.pickupIcon;

            playerAnimations.ChangeAnimationState(weaponData.holdingAnimationName);

            playerAttack.playerIsUnarmed = false;
            
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

            playerInteract.pickupIconImage.sprite = null;

            playerAnimations.ChangeAnimationState();

            playerAttack.playerIsUnarmed = true;
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }

    protected override void Update() {
        base.Update();
        
        if (isHeld) {
            if (Input.GetMouseButtonDown(0)) {
                StartCoroutine(Swing(weaponData.attackSpeed));
            }

            if (Input.GetMouseButton(1)) {
                StartCoroutine(Block(true));
            }
            else if (Input.GetMouseButtonUp(1)) {
                StartCoroutine(Block(false));
            }
        }
    }
    private IEnumerator Swing(float duration) {
        isAttacking = true;

        playerInteract.playerAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(duration);

        isAttacking = false;
    }
    private IEnumerator Block(bool hasBlocked) {
        isBlocking = hasBlocked;
        
        playerInteract.playerAnimator.SetBool("Block", isBlocking);
        
        yield return null;
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
