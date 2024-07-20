using System.Collections;
using UnityEngine;

public class Swingable : PickableLimb
{
    [Header("Main")]
    [SerializeField] private bool isHeld = false;
    public bool isAttacking = false;
    public bool isBlocking = false;

    [Header("Damage Properties")]
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private float damageDuration = 0.01f;
    [SerializeField] private float lightAttackDuration = 1f;

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

            transform.SetParent(playerHand);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            animator.enabled = true;

            itemRigidbody.isKinematic = false;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;
            
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
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }

    protected override void Update() {
        base.Update();
        
        if (isHeld) {
            if (Input.GetMouseButtonDown(0)) {
                StartCoroutine(Swing(lightAttackDuration));
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
                                float scaledDamageAmount = damageAmount * data.transformDamageMultiplier;

                                data.transformCurrentHealth -= scaledDamageAmount;
                            
                                EnemyHealth enemyHealth = Helper.GetComponentInParentByTag<EnemyHealth>(other.transform, "Enemy");
                                if (enemyHealth != null) {
                                    enemyHealth.DiffHealth(scaledDamageAmount, damageDuration);
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

                GetComponent<AudioSource>().PlayOneShot(
                    impactEffectData.impactClip, 
                    impactEffectData.impactVolume
                );
                Helper.CameraShake(
                    impactEffectData.hurtShakeMagnitude, 
                    impactEffectData.hurtShakeDuration, 
                    impactEffectData.hurtShakeMultiplier
                );
            }
        }
    }
}
