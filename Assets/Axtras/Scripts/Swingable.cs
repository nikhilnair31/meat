using System.Collections;
using UnityEngine;

public class Swingable : MonoBehaviour 
{
    private PlayerPickups playerPickups;
    private Animator playerAnimator;

    [Header("Swingable Properties")]
    [SerializeField] private float lightAttackDuration = 0.2f;
    [SerializeField] private float heavyAttackDuration = 1.0f;
    [SerializeField] private float attackCooldown = 0.5f;
    public bool isAttacking = false;
    public bool isBlocking = false;
    private float attackTimer = 0f;

    [Header("Damage Properties")]
    [SerializeField] private int damageAmount = 20;

    [Header("Weapon Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int limbStrengthDamage = 5;
    private Material weaponMaterial;
    private Color originalColor;

    [Header("Effects")]
    [SerializeField] private float hurtShakeMagnitude = 6.0f;
    [SerializeField] private float hurtShakeDuration = 0.3f;

    private void Start() {
        playerPickups = FindObjectOfType<PlayerPickups>();

        weaponMaterial = GetComponent<Renderer>().material;
        playerAnimator = GetComponent<Animator>();

        Init();
    }
    // Init Related
    private void Init() {
        currentHealth = maxHealth;
        originalColor = weaponMaterial.color;
    }

    private void Update() {
        HandleSwingable();
    }

    private void HandleSwingable()
    {
        attackTimer -= Time.deltaTime;

        if (playerPickups.currentPlayerPickupItem.type == PlayerPickupItems.PickupType.Swingable) {
            if (attackTimer > 0) {
                return;
            }

            if (Input.GetMouseButtonDown(0)) {
                StartCoroutine(PerformAttack(lightAttackDuration));
                attackTimer = attackCooldown;
            }
            if (Input.GetMouseButtonDown(0) && Input.GetMouseButton(0)) {
                StartCoroutine(PerformAttack(heavyAttackDuration));
                attackTimer = attackCooldown + heavyAttackDuration - lightAttackDuration;
            }

            if (Input.GetMouseButton(1)) {
                PerformBlock(true);
            }
            else if (Input.GetMouseButtonUp(1)) {
                PerformBlock(false);
            }
        }
    }

    // Swingable Related
    IEnumerator PerformAttack(float duration)
    {
        isAttacking = true;

        // Add attack animation or effects here
        playerAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(duration);

        isAttacking = false;
    }
    private void PerformBlock(bool hasBlocked)
    {
        isBlocking = hasBlocked;
        playerAnimator.SetBool("Block", isBlocking);
    }


    private void OnTriggerEnter(Collider other) {
        SwingableImpacting(other);
    }
    private void SwingableImpacting(Collider other) {
        // Debug.Log($"Weapon collided with {other.name} of tag {other.tag}");

        if(isAttacking) {
            if (other.CompareTag("Limb")) {
                TransformCollector transformCollector = Helper.GetComponentInParentByTag<TransformCollector>(other.transform, "Enemy");
                if (transformCollector != null) {
                    foreach (TransformData data in transformCollector.transformDataList) {
                        if(data.transformName.Contains(other.name)) {
                            data.transformCurrentHealth -= damageAmount;
                        }
                    }
                }
                else {
                    Debug.LogError("TransformCollector not found on " + other.name);
                }
            }

            Helper.CameraShake(hurtShakeMagnitude, hurtShakeDuration);
            ReduceWeaponHealth(limbStrengthDamage);
        }
    } 

    private void ReduceWeaponHealth(int amount) {
        currentHealth -= amount;
        Debug.Log($"ReduceWeaponHealth currentHealth: {currentHealth}");

        if (currentHealth <= 0) {
            // Handle weapon destruction
            playerPickups.DisableAllPickups();
        }
        else {
            // Handle weapon damage effect
            float healthRatio = (float) currentHealth / maxHealth;
            weaponMaterial.color = Color.Lerp(Color.black, originalColor, healthRatio);
        }
    } 
}