using Cinemachine;
using UnityEngine;

public class Throwable : PickableLimb
{
    [Header("Main")]
    [SerializeField] private bool isHeld = false;
    [SerializeField] private bool isThrown = false;
    [SerializeField] private float throwForce = 20f;

    [Header("Damage Properties")]
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private float damageDuration = 0.01f;

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

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = false;
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

        Vector3 throwPoint = Helper.CameraCenterTargetPoint();
        Vector3 throwVelocity = (throwPoint - transform.position).normalized * throwForce;
        itemRigidbody.velocity = throwVelocity;
            
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