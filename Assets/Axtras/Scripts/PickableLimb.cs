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
    [SerializeField] private int reduceByImpactDurability = 2;
    [SerializeField] private int reduceByDecayDurability = 2;
    [SerializeField] private float durabilityDecayRate = 1f;
    [SerializeField] private int maxDurability = 20;
    [SerializeField] private int currentDurability;
    private Material weaponMaterial;
    private Color originalColor;

    private void Start() {
        playerInteract = GameObject.Find("Player").GetComponent<PlayerInteract>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        itemCollider = GetComponent<Collider>();
        itemRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        weaponMaterial = GetComponent<Renderer>().material;

        originalColor = weaponMaterial.color;
        currentDurability = maxDurability;

        StartCoroutine(DecayDurabilityOverTime());
    }

    public void ReduceDurabilityByCollision() {
        // Decrease durability on collision
        ReduceDurability(reduceByImpactDurability);
    }
    public IEnumerator DecayDurabilityOverTime() {
        while (true) {
            // Decrease durability continuously
            yield return new WaitForSeconds(durabilityDecayRate);
            ReduceDurability(reduceByDecayDurability);
        }
    }
    private void ReduceDurability(int reduceDurabulityAmount) {
        currentDurability -= reduceDurabulityAmount;
        UpdateWeaponColor();

        if (currentDurability <= 0) {
            Break();
        }
    }
    private void UpdateWeaponColor() {
        float healthRatio = (float)currentDurability / maxDurability;
        weaponMaterial.color = Color.Lerp(Color.black, originalColor, healthRatio);
    }
    private void Break() {
        Debug.Log($"Item {gameObject.name} broke!");

        Destroy(gameObject);
    }
}