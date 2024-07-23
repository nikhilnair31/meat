using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour 
{
    private PlayerAnimations playerAnimations;

    [Header("General Properties")]
    [SerializeField] private Transform raycastSourceTranform;
    public bool playerIsUnarmed = true;

    [Header("Attack Properties")]
    public bool readyToAttack = true;
    public bool isAttacking = false;
    [SerializeField] private int attackCount = 1;

    [Header("Defend Properties")]
    public bool isBlocking = false;

    [Header("Weapon Properties")]
    public MeleeWeaponData meleeWeaponData;

    [Header("UI Properties")]
    [SerializeField] private Image weaponIconImage;

    [Header("Animation Properties")]
    public const string IDLE = "Idle";
    public const string ATTACK1 = "Punch L";
    public const string ATTACK2 = "Punch R";
    public const string BLOCK = "Block";

    private void Start() {
        playerAnimations = GetComponent<PlayerAnimations>();

        InitUI();
    }
    private void InitUI() {
        if (meleeWeaponData.weaponIcon != null) {
            weaponIconImage.sprite = meleeWeaponData.weaponIcon;
        }
    }

    private void Update() {
        if (playerIsUnarmed) {
            if (Input.GetMouseButtonDown(0)) {
                Attack();
            }
            if (Input.GetMouseButtonDown(1)) {
                Block();
            }
        }
    }

    private void Attack() {
        if(!readyToAttack || isAttacking) return;

        readyToAttack = false;
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
        readyToAttack = true;
        isAttacking = false;
        playerAnimations.ChangeAnimationState(IDLE);
    }

    private void Block() {
        if (!isBlocking) {
            isBlocking = true;

            playerAnimations.ChangeAnimationState(BLOCK);
        }
        else {
            isBlocking = false;

            playerAnimations.ChangeAnimationState(IDLE);
        }
    }
}