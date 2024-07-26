using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour 
{
    private PlayerAnimations playerAnimations;
    private PlayerInteract playerInteract;
    private PlayerHealth playerHealth;

    [Header("General Properties")]
    [SerializeField] private Transform raycastSourceTranform;

    [Header("Unarmed Properties")]
    public MeleeItemData meleeItemData;
    [SerializeField] private bool isAttacking = false;
    public bool isBlocking = false;
    [SerializeField] private int attackCount = 1;

    [Header("Consuming Settings")]
    public bool isConsuming = false;
    public float speedReductionMultiplier = 1f;
    private float holdTime;

    [Header("Throw Settings")]
    [SerializeField] private bool isThrowing = false;

    [Header("UI Properties")]
    [SerializeField] private Image weaponIconImage;

    [Header("Animation Properties")]
    public const string IDLE = "Idle";

    private void Start() {
        playerAnimations = GetComponent<PlayerAnimations>();
        playerInteract = GetComponent<PlayerInteract>();
        playerHealth = GetComponent<PlayerHealth>();

        weaponIconImage.sprite = meleeItemData.icon;
    }

    private void Update() {
        if (playerInteract.currentHeldItemType == Pickable.PickableType.None) {
            if (Input.GetMouseButtonDown(0)) {
                UnarmedAttack();
            }

            if (Input.GetMouseButtonDown(1)) {
                UnarmedBlock(true);
            }
            if (Input.GetMouseButtonUp(1)) {
                UnarmedBlock(false);
            }
        }
        else if (playerInteract.currentHeldItemType == Pickable.PickableType.Swingable) {
            if (Input.GetMouseButtonDown(0)) {
                SwingableAttack();
            }

            if (Input.GetMouseButtonDown(1)) {
                SwingableBlock(true);
            }
            if (Input.GetMouseButtonUp(1)) {
                SwingableBlock(false);
            }
        }
        else if (playerInteract.currentHeldItemType == Pickable.PickableType.Consumable) {
            if(Input.GetMouseButton(0)) {
                HandleConsumption();
            }
            else if(Input.GetMouseButtonUp(0)) {
                ResetConsumptionOnMouseRelease();
            }
        }
        else if (playerInteract.currentHeldItemType == Pickable.PickableType.Throwable) {
            if (Input.GetMouseButtonDown(0)) {
                Throw();
            }
        }
    }

    #region Unarmed
    private void UnarmedAttack() {
        if(isAttacking) return;

        isAttacking = true;
        isBlocking = false;

        Invoke(nameof(UnarmedAttackReset), meleeItemData.attackResetInTime);
        Invoke(nameof(UnarmedAttackRaycast), meleeItemData.attackActionInTime);

        if(attackCount == 1) {
            playerAnimations.ChangeAnimationState(meleeItemData.attackAnimName1);
            attackCount++;
        }
        else if(attackCount == 2) {
            playerAnimations.ChangeAnimationState(meleeItemData.attackAnimName2);
            attackCount = 1;
        }
    }
    private void UnarmedAttackRaycast() {
        AttackRaycast(
            meleeItemData.attackLayer
            , meleeItemData.attackRange
            , meleeItemData.damageAmount
            , meleeItemData.damageDuration
            , meleeItemData.knockbackForce
            , meleeItemData.impactEffectDatas
        );
    }
    private void UnarmedAttackReset() {
        isAttacking = false;
        playerAnimations.ChangeAnimationState(meleeItemData.idleAnimName);
    }

    private void UnarmedBlock(bool doBlock) {
        if (doBlock) {
            isBlocking = true;
            playerAnimations.ChangeAnimationState(meleeItemData.block);
        }
        else {
            isBlocking = false;
            playerAnimations.ChangeAnimationState(IDLE);
        }
    }
    #endregion

    #region Swingable
    private void SwingableAttack() {
        if(isAttacking) return;

        isAttacking = true;
        isBlocking = false;
        
        Swingable swingable = playerInteract.currentHeldItem.GetComponent<Swingable>();

        Invoke(nameof(SwingableAttackReset), swingable.itemData.attackResetInTime);
        Invoke(nameof(SwingableAttackRaycast), swingable.itemData.attackActionInTime);

        if(attackCount == 1) {
            playerAnimations.ChangeAnimationState(swingable.itemData.attackAnimName1);
            attackCount++;
        }
        else if(attackCount == 2) {
            playerAnimations.ChangeAnimationState(swingable.itemData.attackAnimName2);
            attackCount = 1;
        }
    }
    private void SwingableAttackRaycast() {
        Swingable swingable = playerInteract.currentHeldItem.GetComponent<Swingable>();
        AttackRaycast(
            swingable.itemData.attackLayer
            , swingable.itemData.attackRange
            , swingable.itemData.damageAmount
            , swingable.itemData.damageDuration
            , swingable.itemData.knockbackForce
            , swingable.itemData.impactEffectDatas
        );
    }
    private void SwingableAttackReset() {
        isAttacking = false;

        if (playerInteract.currentHeldItem.TryGetComponent<Swingable>(out var swingable)) {
            playerAnimations.ChangeAnimationState(swingable.itemData.idleAnimName);
        }
        else {
            playerAnimations.ChangeAnimationState(IDLE);
        }
    }

    private void SwingableBlock(bool doBlock) {
        Swingable swingable = playerInteract.currentHeldItem.GetComponent<Swingable>();

        if (doBlock) {
            isBlocking = true;
            playerAnimations.ChangeAnimationState(swingable.itemData.block);
        }
        else {
            isBlocking = false;
            playerAnimations.ChangeAnimationState(IDLE);
        }
    }
    #endregion

    #region Consumable
    private void HandleConsumption() {
        Consumable consumable = playerInteract.currentHeldItem.GetComponent<Consumable>();

        holdTime += Time.deltaTime;
        playerAnimations.ChangeAnimationState(consumable.itemData.consumingAnimName);
        playerInteract.fillableCursorImage.fillAmount = holdTime / consumable.itemData.consumeTime;
        speedReductionMultiplier = consumable.itemData.speedReductionMultiplier;
        isConsuming = true;

        if (holdTime >= consumable.itemData.consumeTime) {
            Debug.Log("Player healed!");
            
            playerHealth.AddHealth(
                consumable.itemData.healAmount, 
                consumable.itemData.healTime
            );
            ResetConsumptionOnMouseRelease();

            consumable.Drop();
            Destroy(consumable.gameObject);
        }
    }
    private void ResetConsumptionOnMouseRelease() {
        holdTime = 0f;
        playerInteract.fillableCursorImage.fillAmount = 0f;
        speedReductionMultiplier = 1f;
        isConsuming = false;
    }
    #endregion

    #region Throwable
    private void Throw() {
        if(isThrowing) return;

        isThrowing = true;

        Throwable throwable = playerInteract.currentHeldItem.GetComponent<Throwable>();
        ThrowableItemData throwableItemData = throwable.itemData;

        Invoke(nameof(ResetThrow), throwableItemData.throwSpeed);
        Invoke(nameof(ThrowPhysics), throwableItemData.throwDelay);

        playerAnimations.ChangeAnimationState(throwableItemData.throwingAnimName);
    }
    private void ThrowPhysics() {
        Throwable throwable = playerInteract.currentHeldItem.GetComponent<Throwable>();

        throwable.Throw();
    }
    private void ResetThrow() {
        isThrowing = false;
        playerAnimations.ChangeAnimationState(IDLE);
    }
    #endregion

    #region General
    private void AttackRaycast(LayerMask layer, float range, float damageAmount, float damageDuration, float knockbackForce, List<ImpactEffectData> impactEffectDatas) {
        Debug.DrawRay(raycastSourceTranform.position, raycastSourceTranform.forward * range, Color.red, 1f);

        if (Physics.Raycast(raycastSourceTranform.position, raycastSourceTranform.forward, out RaycastHit hit, range, layer))
        {
            Debug.Log($"SwingableAttackRaycast hit name {hit.collider.name} of tag {hit.collider.tag}");
            if (hit.collider.CompareTag("Limb")) {
                LimbImpact(hit.transform, damageAmount, damageDuration);
            }
            else {
                OtherImpact(hit.transform, hit.point, knockbackForce);
            }

            foreach (ImpactEffectData impactEffectData in impactEffectDatas) {
                ShowEffects(hit, impactEffectData);
            }
        }
        else {
            // Debug.Log("Did not hit anything");
        }
    }
    private void LimbImpact(Transform hit, float damageAmount, float damageDuration) {
        Debug.Log($"LimbImpact: {hit.name}");
        
        TransformCollector transformCollector = Helper.GetComponentInParentByTag<TransformCollector>(hit, "Enemy");
        if (transformCollector != null) {
            foreach (TransformData data in transformCollector.transformDataList) {
                if (data.transformName.Contains(hit.name)) {
                    float scaledDamageAmount = damageAmount * data.transformDamageMultiplier;

                    data.transformCurrentHealth -= scaledDamageAmount;

                    EnemyHealth enemyHealth = Helper.GetComponentInParentByTag<EnemyHealth>(hit, "Enemy");
                    if (enemyHealth != null) {
                        enemyHealth.DiffHealth(scaledDamageAmount, damageDuration);
                    }
                }
            }
        }
        else {
            Debug.LogError($"TransformCollector not found on {hit.name}");
        }
    }
    private void OtherImpact(Transform hit, Vector3 hitPoint, float knockbackForce) {
        Debug.Log($"OtherImpact: {hit.name}");

        if (hit.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            rb.AddForceAtPosition(
                raycastSourceTranform.forward * knockbackForce, 
                hitPoint, 
                ForceMode.Impulse
            );
        }
    }
    private void ShowEffects(RaycastHit hit, ImpactEffectData impactEffectData) {
        if (((1 << hit.transform.gameObject.layer) & impactEffectData.layer.value) != 0) {
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
                hit.point,
                Quaternion.identity
            );
            if (impactEffectData.destroyAfterInstantiation) {
                Destroy(impactParticle, impactEffectData.destroyDelay);
            }
        }
    }
    #endregion

}