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
    public MeleeWeaponData meleeWeaponData;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isBlocking = false;
    [SerializeField] private int attackCount = 1;

    [Header("Consuming Settings")]
    public bool isConsuming = false;
    public float speedReductionMultiplier = 1f;
    private float holdTime;

    [Header("UI Properties")]
    [SerializeField] private Image weaponIconImage;

    [Header("Animation Properties")]
    public const string IDLE = "Idle";
    public const string ATTACK1 = "Punch L";
    public const string ATTACK2 = "Punch R";
    public const string ATTACK3 = "Kick";
    public const string BLOCK = "Block";

    private void Start() {
        playerAnimations = GetComponent<PlayerAnimations>();
        playerInteract = GetComponent<PlayerInteract>();
        playerHealth = GetComponent<PlayerHealth>();

        InitActionUI();
    }
    private void InitActionUI() {
        if (meleeWeaponData.weaponIcon != null) {
            weaponIconImage.sprite = meleeWeaponData.weaponIcon;
        }
    }

    private void Update() {
        if (playerInteract.currentHeldItemType == Pickable.PickableType.None) {
            if (Input.GetMouseButtonDown(0)) {
                Attack();
            }

            if (Input.GetMouseButtonDown(1)) {
                Block(true);
            }
            if (Input.GetMouseButtonUp(1)) {
                Block(false);
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
        else if (playerInteract.currentHeldItemType == Pickable.PickableType.Swingable) {
            if (Input.GetMouseButtonDown(0)) {
                Attack();
            }

            if (Input.GetMouseButtonDown(1)) {
                Block(true);
            }
            if (Input.GetMouseButtonUp(1)) {
                Block(false);
            }
        }
    }

    private void Attack() {
        if(isAttacking) return;

        isAttacking = true;
        isBlocking = false;

        Invoke(nameof(ResetAttack), meleeWeaponData.attackSpeed);
        Invoke(nameof(AttackRaycast), meleeWeaponData.attackDelay);

        if(attackCount == 1) {
            playerAnimations.ChangeAnimationState(ATTACK1);
            attackCount++;
        }
        else if(attackCount == 2) {
            playerAnimations.ChangeAnimationState(ATTACK2);
            attackCount = 1;
        }
    }
    private void AttackRaycast() {
        // Debug.Log("AttackRaycast");

        Debug.DrawRay(raycastSourceTranform.position, raycastSourceTranform.forward * meleeWeaponData.attackRange, Color.red, 1f);

        RaycastHit hit;
        if(Physics.Raycast(raycastSourceTranform.position, raycastSourceTranform.forward, out hit, meleeWeaponData.attackRange, meleeWeaponData.attackLayer)) {
            // Debug.Log($"hit name {hit.collider.name} of tag {hit.collider.tag}");
            if (hit.collider.CompareTag("Limb")) {
                TransformCollector transformCollector = Helper.GetComponentInParentByTag<TransformCollector>(hit.transform, "Enemy");
                if (transformCollector != null) {
                    foreach (TransformData data in transformCollector.transformDataList) {
                        if (data.transformName.Contains(hit.collider.name)) {
                            float scaledDamageAmount = meleeWeaponData.damageAmount * data.transformDamageMultiplier;

                            data.transformCurrentHealth -= scaledDamageAmount;

                            EnemyHealth enemyHealth = Helper.GetComponentInParentByTag<EnemyHealth>(hit.transform, "Enemy");
                            if (enemyHealth != null) {
                                enemyHealth.DiffHealth(scaledDamageAmount, meleeWeaponData.damageDuration);
                            }
                        }
                    }
                } 
                else {
                    // Debug.LogError($"TransformCollector not found on {hit.collider.name}");
                }
            } 
            else {
                // Debug.Log($"Player hit something else! {hit.collider.name}");
            }

            GameObject impactParticlePrefab = meleeWeaponData.impactEffectData.impactParticlePrefab;
            if (impactParticlePrefab != null && hit.collider != null) {
                GameObject impactParticle = Instantiate(impactParticlePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactParticle, 2f);
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
        else {
            // Debug.Log("Did not hit anything");
        }
    }
    private void ResetAttack() {
        isAttacking = false;
        playerAnimations.ChangeAnimationState(IDLE);
    }

    private void Block(bool doBlock) {
        if (doBlock) {
            isBlocking = true;
            playerAnimations.ChangeAnimationState(BLOCK);
        }
        else {
            isBlocking = false;
            playerAnimations.ChangeAnimationState(IDLE);
        }
    }

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

            consumable.Drop(false);
        }
    }
    private void ResetConsumptionOnMouseRelease() {
        holdTime = 0f;
        playerInteract.fillableCursorImage.fillAmount = 0f;
        speedReductionMultiplier = 1f;
        isConsuming = false;
    }

    private void Throw() {
        Throwable throwable = playerInteract.currentHeldItem.GetComponent<Throwable>();

        throwable.Throw();
    }
}