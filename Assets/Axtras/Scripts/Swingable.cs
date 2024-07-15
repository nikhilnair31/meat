using System.Collections;
using UnityEngine;

public class Swingable : Interactable
{
    private PlayerInteract playerInteract;
    private Animator swingableAnimator;

    [Header("Main")]
    [SerializeField] private bool isHeld = false;
    private Transform playerHand;
    private Rigidbody itemRigidbody;
    private Collider itemCollider;
    public bool isAttacking = false;
    public bool isBlocking = false;

    [Header("Damage Properties")]
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private float lightAttackDuration = 1f;

    [Header("Durability Properties")]
    [SerializeField] private int reduceByDecayDurability = 2;
    [SerializeField] private float durabilityDecayRate = 1f;
    [SerializeField] private int reduceByImpactDurability = 5;
    [SerializeField] private int maxDurability = 20;
    [SerializeField] private int currentDurability;
    private Material weaponMaterial;
    private Color originalColor;

    [Header("Effects")]
    [SerializeField] private float hurtShakeMagnitude = 6.0f;
    [SerializeField] private float hurtShakeDuration = 0.3f;

    private void Start() {
        playerInteract = GameObject.Find("Player").GetComponent<PlayerInteract>();

        itemRigidbody = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
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
            playerInteract.playerAnimator = swingableAnimator;

            transform.SetParent(playerHand);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            swingableAnimator.enabled = true;

            itemRigidbody.isKinematic = false;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = true;
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
            if (Input.GetMouseButtonDown(0)) {
                StartCoroutine(Swing(lightAttackDuration));
            }

            if (Input.GetMouseButton(1)) {
                StartCoroutine(Block(true));
            }
            else if (Input.GetMouseButtonUp(1)) {
                StartCoroutine(Block(false));
            }
        }
    }
    private IEnumerator Swing(float duration) {
        isAttacking = true;

        playerInteract.playerAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(duration);

        isAttacking = false;
    }
    private IEnumerator Block(bool hasBlocked) {
        isBlocking = hasBlocked;
        
        playerInteract.playerAnimator.SetBool("Block", isBlocking);
        
        yield return null;
    }

    private void OnCollisionEnter(Collision other) {
        if (isHeld) {
            if(isAttacking) {
                if (other.collider.CompareTag("Limb")) {
                    TransformCollector transformCollector = Helper.GetComponentInParentByTag<TransformCollector>(other.transform, "Enemy");
                    if (transformCollector != null) {
                        foreach (TransformData data in transformCollector.transformDataList) {
                            if(data.transformName.Contains(other.collider.name)) {
                                data.transformCurrentHealth -= damageAmount;
                            }
                        }
                    }
                    else {
                        Debug.LogError("TransformCollector not found on " + other.collider.name);
                    }
                }
                else {
                    // Deal damage to the player
                    Debug.Log("Player hit something else!");
                }

                // Decrease durability on collision
                ReduceDurabilityByCollision(reduceByImpactDurability);

                Helper.CameraShake(hurtShakeMagnitude, hurtShakeDuration);
            }
        }
    }

    private IEnumerator DecayDurabilityOverTime(int reduceByDecayDurability) {
        while (true) {
            // Decrease durability continuously
            yield return new WaitForSeconds(durabilityDecayRate);
            ReduceDurability(reduceByDecayDurability);
        }
    }
    private void ReduceDurabilityByCollision(int reduceByImpactDurability) {
        // Decrease durability on collision
        ReduceDurability(reduceByImpactDurability);
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
        Debug.Log($"Swingable item {gameObject.name} broke!");
        Destroy(gameObject);
    }
}
