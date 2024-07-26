using UnityEngine;

public class Pickable : Interactable 
{
    internal Rigidbody itemRigidbody;
    internal Collider itemCollider;
    internal bool isHeld = false;

    public enum PickableType {
        None,
        Consumable,
        Throwable,
        Swingable
    }

    [Header("Type Properties")]
    [SerializeField] private PickableType pickableType;
    
    [Header("Durability Properties")]
    [SerializeField] private DurabilityData durabilityData;
    private Material weaponMaterial;
    private Color originalColor;
    private float currentDurability;
    private float decayTimer;

    public override void Start() {
        base.Start();

        itemCollider = GetComponent<Collider>();
        itemRigidbody = GetComponent<Rigidbody>();

        weaponMaterial = GetComponent<Renderer>().material;

        originalColor = weaponMaterial.color;
        currentDurability = durabilityData.maxDurability;
    }
    public override void Update() {
        base.Update();
        
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
            Debug.Log($"Item {gameObject.name} broke!");
            if (isHeld) {
                playerInteract.currentHeldItem = null;
                playerInteract.currentHeldItemType = PickableType.None;
                playerInteract.pickupIconImage.sprite = playerAction.meleeItemData.icon;

                playerAnimations.ChangeAnimationState();
            }
            Destroy(gameObject);
        }
    }
    private void UpdateWeaponColor() {
        float healthRatio = currentDurability / durabilityData.maxDurability;
        weaponMaterial.color = Color.Lerp(durabilityData.finalColor, originalColor, healthRatio);
    }
}