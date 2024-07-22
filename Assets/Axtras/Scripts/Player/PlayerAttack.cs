using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour 
{
    private PlayerAnimations playerAnimations;

    public const string ATTACK1 = "Punch L";
    public const string ATTACK2 = "Punch R";

    [Header("Components Properties")]
    [SerializeField] private Transform raycastSourceTranform;

    [Header("Attack Properties")]
    bool readyToAttack = true;
    public bool isAttacking = false;
    public bool playerIsUnarmed = true;
    [SerializeField] private int attackCount = 1;

    [Header("Weapon Properties")]
    [SerializeField] private MeleeWeaponData meleeWeaponData;

    [Header("UI Properties")]
    [SerializeField] private Image weaponIconImage;

    private void Start() {
        if (playerAnimations == null) {
            playerAnimations = GetComponent<PlayerAnimations>();
        }

        InitUI();
    }
    private void InitUI() {
        if (meleeWeaponData.weaponIcon != null) {
            weaponIconImage.sprite = meleeWeaponData.weaponIcon;
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Attack();
        }
    }    

    public void Attack() {
        if(!playerIsUnarmed) return;

        if(!readyToAttack || isAttacking) return;

        readyToAttack = false;
        isAttacking = true;

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
        readyToAttack = true;
    }
}