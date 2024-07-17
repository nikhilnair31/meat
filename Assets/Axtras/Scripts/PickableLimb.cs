using System.Collections;
using UnityEngine;

public class PickableLimb : Interactable 
{
    [Header("General Properties")]
    public PlayerInteract playerInteract;
    public PlayerHealth playerHealth;
    public Transform playerHand;
    public Rigidbody itemRigidbody;
    public Collider itemCollider;
    public Animator animator;
    
    [Header("Durability Properties")]
    [SerializeField] private float reduceByImpactDurability = 2f;
    [SerializeField] private float durabilityDecayInTime = 5f;
    [SerializeField] private float maxDurability = 20;
    [SerializeField] private float currentDurability;
    private Material weaponMaterial;
    private Color originalColor;
    private float decayTimer;

    private void Start() {
        playerInteract = GameObject.Find("Player").GetComponent<PlayerInteract>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        itemCollider = GetComponent<Collider>();
        itemRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        weaponMaterial = GetComponent<Renderer>().material;

        originalColor = weaponMaterial.color;
        currentDurability = maxDurability;
    }
    protected virtual void Update() {
        DecayDurabilityOverTime();
    }

    public void ReduceDurabilityByCollision() {
        // Decrease durability on collision
        ReduceDurability(reduceByImpactDurability);
    }
    private void DecayDurabilityOverTime() {
        if (currentDurability > 0f) {
            decayTimer += Time.deltaTime;
            float decayAmount = Time.deltaTime * (maxDurability / durabilityDecayInTime);
            ReduceDurability(decayAmount);
        }
    }
    private void ReduceDurability(float reduceDurabilityAmount) {
        currentDurability -= reduceDurabilityAmount;
        currentDurability = Mathf.Clamp(currentDurability, 0, maxDurability);
        UpdateWeaponColor();

        if (currentDurability <= 0) {
            Break();
        }
    }
    private void UpdateWeaponColor() {
        float healthRatio = currentDurability / maxDurability;
        weaponMaterial.color = Color.Lerp(Color.black, originalColor, healthRatio);
    }
    private void Break() {
        Debug.Log($"Item {gameObject.name} broke!");

        Destroy(gameObject);
    }
}