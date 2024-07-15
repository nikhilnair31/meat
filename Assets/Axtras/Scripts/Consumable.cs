using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Consumable : Interactable
{
    private PlayerInteract playerInteract;
    private PlayerHealth playerHealth;
    private Image fillableCursorImage;
    private Animator swingableAnimator;

    [Header("Main")]
    [SerializeField] private bool isHeld = false;
    private Transform playerHand;
    private Collider itemCollider;
    private Rigidbody itemRigidbody;

    [Header("Heal Properties")]
    [SerializeField] public int healAmount = 20;
    [SerializeField] public float healTime = 3f;
    [SerializeField] public float consumeTime = 3f;
    [SerializeField] private float holdTime = 0f;

    [Header("Durability Properties")]
    [SerializeField] private int reduceByDecayDurability = 2;
    [SerializeField] private float durabilityDecayRate = 1f;
    [SerializeField] private int maxDurability = 20;
    [SerializeField] private int currentDurability;
    private Material weaponMaterial;
    private Color originalColor;

    void Start() {
        playerInteract = GameObject.Find("Player").GetComponent<PlayerInteract>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        itemCollider = GetComponent<Collider>();
        itemRigidbody = GetComponent<Rigidbody>();
        swingableAnimator = GetComponent<Animator>();
        weaponMaterial = GetComponent<Renderer>().material;

        originalColor = weaponMaterial.color;
        currentDurability = maxDurability;

        StartCoroutine(DecayDurabilityOverTime(reduceByDecayDurability));
    }

    public override void Interact() {
        Pickup();
    }
    public override void Pickup() {
        if (!isHeld) {
            isHeld = true;
            
            playerHand = playerInteract.playerInteractHolder;
            fillableCursorImage = playerInteract.fillableCursorImage;
            playerInteract.playerAnimator = swingableAnimator;

            transform.SetParent(playerHand);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            swingableAnimator.enabled = true;

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = false;
            itemCollider.isTrigger = false;
            
            ShowUI = false;
        }
        else {
            Debug.Log($"Item {gameObject.name} is already held");
        }
    }
    public override void Drop() {
        if (isHeld) {
            isHeld = false;

            transform.SetParent(null);
            
            itemRigidbody.isKinematic = false;
            itemRigidbody.useGravity = true;

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }

    private void Update() {
        if (isHeld) {
            if(Input.GetMouseButton(0)) {
                HandleConsumption();
            }
            else if(Input.GetMouseButtonUp(0)) {
                ResetConsumptionOnMouseRelease();
            }
        }
    }

    void HandleConsumption() {
        if (Input.GetMouseButton(0)) {
            holdTime += Time.deltaTime;
            fillableCursorImage.fillAmount = holdTime / consumeTime;

            if (holdTime >= consumeTime) {
                Consume();
            }
        }
    }

    void ResetConsumptionOnMouseRelease() {
        holdTime = 0f;
        fillableCursorImage.fillAmount = 0f;
    }

    void Consume() {
        Debug.Log("Player healed!");
        
        playerHealth.AddHealth(healAmount, healTime);
        ResetConsumptionOnMouseRelease();

        Destroy(gameObject);
    }

    private IEnumerator DecayDurabilityOverTime(int reduceByDecayDurability) {
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
        // Handle item breaking logic
        Debug.Log($"Throwable item {gameObject.name} broke!");
        Destroy(gameObject);
    }
}