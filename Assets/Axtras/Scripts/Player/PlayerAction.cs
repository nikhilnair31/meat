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
    public const string BLOCK = "Block";

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
        Debug.DrawRay(raycastSourceTranform.position, raycastSourceTranform.forward * meleeItemData.attackRange, Color.red, 1f);

        if (Physics.Raycast(raycastSourceTranform.position, raycastSourceTranform.forward, out RaycastHit hit, meleeItemData.attackRange, meleeItemData.attackLayer))
        {
            Debug.Log($"UnarmedAttackRaycast hit name {hit.collider.name} of tag {hit.collider.tag}");
            if (hit.collider.CompareTag("Limb"))
            {
                TransformCollector transformCollector = Helper.GetComponentInParentByTag<TransformCollector>(hit.transform, "Enemy");
                if (transformCollector != null)
                {
                    foreach (TransformData data in transformCollector.transformDataList)
                    {
                        if (data.transformName.Contains(hit.collider.name))
                        {
                            float scaledDamageAmount = meleeItemData.damageAmount * data.transformDamageMultiplier;

                            data.transformCurrentHealth -= scaledDamageAmount;

                            EnemyHealth enemyHealth = Helper.GetComponentInParentByTag<EnemyHealth>(hit.transform, "Enemy");
                            if (enemyHealth != null)
                            {
                                enemyHealth.DiffHealth(scaledDamageAmount, meleeItemData.damageDuration);
                            }
                        }
                    }
                }
                else
                {
                    // Debug.LogError($"TransformCollector not found on {hit.collider.name}");
                }
            }
            else
            {
                // Debug.Log($"Player hit something else! {hit.collider.name}");
            }

            GameObject impactParticlePrefab = meleeItemData.impactEffectData.impactParticlePrefab;
            if (impactParticlePrefab != null && hit.collider != null)
            {
                GameObject impactParticle = Instantiate(impactParticlePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactParticle, 2f);
            }

            Helper.PlayOneShotWithRandPitch(
                GetComponent<AudioSource>(),
                meleeItemData.impactEffectData.impactClip,
                meleeItemData.impactEffectData.impactVolume,
                meleeItemData.impactEffectData.randPitch
            );
            Helper.CameraShake(
                meleeItemData.impactEffectData.hurtShakeMagnitude,
                meleeItemData.impactEffectData.hurtShakeDuration,
                meleeItemData.impactEffectData.hurtShakeMultiplier
            );
        }
        else
        {
            // Debug.Log("Did not hit anything");
        }
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

        Debug.DrawRay(raycastSourceTranform.position, raycastSourceTranform.forward * swingable.itemData.attackRange, Color.red, 1f);

        if (Physics.Raycast(raycastSourceTranform.position, raycastSourceTranform.forward, out RaycastHit hit, swingable.itemData.attackRange, swingable.itemData.attackLayer))
        {
            Debug.Log($"SwingableAttackRaycast hit name {hit.collider.name} of tag {hit.collider.tag}");
            if (hit.collider.CompareTag("Limb"))
            {
                TransformCollector transformCollector = Helper.GetComponentInParentByTag<TransformCollector>(hit.transform, "Enemy");
                if (transformCollector != null)
                {
                    foreach (TransformData data in transformCollector.transformDataList)
                    {
                        if (data.transformName.Contains(hit.collider.name))
                        {
                            float scaledDamageAmount = swingable.itemData.damageAmount * data.transformDamageMultiplier;

                            data.transformCurrentHealth -= scaledDamageAmount;

                            EnemyHealth enemyHealth = Helper.GetComponentInParentByTag<EnemyHealth>(hit.transform, "Enemy");
                            if (enemyHealth != null)
                            {
                                enemyHealth.DiffHealth(scaledDamageAmount, swingable.itemData.damageDuration);
                            }
                        }
                    }
                }
                else
                {
                    // Debug.LogError($"TransformCollector not found on {hit.collider.name}");
                }
            }
            else
            {
                // Debug.Log($"Player hit something else! {hit.collider.name}");
            }

            GameObject impactParticlePrefab = swingable.itemData.impactEffectData.impactParticlePrefab;
            if (impactParticlePrefab != null && hit.collider != null)
            {
                GameObject impactParticle = Instantiate(impactParticlePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactParticle, 2f);
            }

            Helper.PlayOneShotWithRandPitch(
                GetComponent<AudioSource>(),
                swingable.itemData.impactEffectData.impactClip,
                swingable.itemData.impactEffectData.impactVolume,
                swingable.itemData.impactEffectData.randPitch
            );
            Helper.CameraShake(
                swingable.itemData.impactEffectData.hurtShakeMagnitude,
                swingable.itemData.impactEffectData.hurtShakeDuration,
                swingable.itemData.impactEffectData.hurtShakeMultiplier
            );
        }
        else
        {
            // Debug.Log("Did not hit anything");
        }
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

            consumable.Drop(true);
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
}