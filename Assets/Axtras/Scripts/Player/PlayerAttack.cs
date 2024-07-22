using UnityEngine;

public class PlayerAttack : MonoBehaviour 
{

    public const string IDLE = "Idle";
    public const string ATTACK1 = "Punch L";
    public const string ATTACK2 = "Punch R";
    string currentAnimationState;

    bool readyToAttack = true;

    [Header("Components Properties")]
    [SerializeField] private Transform raycastSourceTranform;
    [SerializeField] private Animator playerAnimator;

    [Header("Attack Properties")]
    public bool isAttacking = false;
    [SerializeField] private int attackCount = 1;
    [SerializeField] private float attackDelay = 0.4f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask attackLayer;

    [Header("Weapon Properties")]
    [SerializeField] private MeleeWeaponData meleeWeaponData;

    private void Start() {
        if (playerAnimator == null) {
            playerAnimator = GetComponentInChildren<Animator>();
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Attack();
        }

        SetAnimations();
    }    

    public void Attack() {
        if(!readyToAttack || isAttacking) return;

        readyToAttack = false;
        isAttacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        if(attackCount == 1) {
            ChangeAnimationState(ATTACK1);
            attackCount++;
        }
        else if(attackCount == 2) {
            ChangeAnimationState(ATTACK2);
            attackCount = 1;
        }
    }
   
    void AttackRaycast() {
        // Debug.Log("AttackRaycast");

        Debug.DrawRay(raycastSourceTranform.position, raycastSourceTranform.forward * attackRange, Color.red, 1f);

        RaycastHit hit;
        if(Physics.Raycast(raycastSourceTranform.position, raycastSourceTranform.forward, out hit, attackRange, attackLayer)) {
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

    void ResetAttack() {
        isAttacking = false;
        readyToAttack = true;
    }

    public void ChangeAnimationState(string newState)  {
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState) {
            return;
        }

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        playerAnimator.CrossFadeInFixedTime(currentAnimationState, 0.1f);
    }

    void SetAnimations() {
        // If player is not isAttacking
        if(!isAttacking) {
            ChangeAnimationState(IDLE);
        }
    }
}