using UnityEngine;

public class PickableLimb : Interactable 
{
    [Header("General Properties")]
    internal PlayerMovementRigidbody playerMovementRigidbody;
    internal PlayerAnimations playerAnimations;
    internal PlayerInteract playerInteract;
    internal PlayerHealth playerHealth;
    internal PlayerAttack playerAttack;
    internal Rigidbody itemRigidbody;
    internal Collider itemCollider;
    internal bool isHeld = false;
    
    [Header("Durability Properties")]
    [SerializeField] private DurabilityData durabilityData;
    private Material weaponMaterial;
    private Color originalColor;
    private float currentDurability;
    private float decayTimer;

    private void Start() {
        playerMovementRigidbody = GameObject.Find("Player").GetComponent<PlayerMovementRigidbody>();
        playerAnimations = GameObject.Find("Player").GetComponent<PlayerAnimations>();
        playerInteract = GameObject.Find("Player").GetComponent<PlayerInteract>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        playerAttack = GameObject.Find("Player").GetComponent<PlayerAttack>();

        itemCollider = GetComponent<Collider>();
        itemRigidbody = GetComponent<Rigidbody>();

        weaponMaterial = GetComponent<Renderer>().material;

        originalColor = weaponMaterial.color;
        currentDurability = durabilityData.maxDurability;
    }
    protected virtual void Update() {
        DecayDurabilityOverTime();
    }

    public void ReduceDurabilityByCollision() {
        ReduceDurability(durabilityData.reduceByImpactDurability);
    }
    private void DecayDurabilityOverTime() {
        if (currentDurability > 0f) {
            decayTimer += Time.deltaTime;
            float decayAmount = Time.deltaTime * (durabilityData.maxDurability / durabilityData.durabilityDecayInTime);
            ReduceDurability(decayAmount);
        }
    }

    private void ReduceDurability(float reduceDurabilityAmount) {
        currentDurability -= reduceDurabilityAmount;
        currentDurability = Mathf.Clamp(currentDurability, 0, durabilityData.maxDurability);
        UpdateWeaponColor();

        if (currentDurability <= 0) {
            Break();
        }
    }
    private void UpdateWeaponColor() {
        float healthRatio = currentDurability / durabilityData.maxDurability;
        weaponMaterial.color = Color.Lerp(Color.black, originalColor, healthRatio);
    }
    private void Break() {
        Debug.Log($"Item {gameObject.name} broke!");

        // FIXME: Genralize this so child classes can use similar for droppping
        if (isHeld) {
            playerInteract.pickupIconImage.sprite = playerAttack.meleeWeaponData.weaponIcon;

            playerAnimations.ChangeAnimationState();

            playerAttack.playerIsUnarmed = true;
        }

        Destroy(gameObject);
    }
}