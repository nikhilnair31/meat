using System.Collections;
using UnityEngine;

public class Throwable : Interactable
{
    private PlayerInteract playerInteract;

    [Header("Main")]
    [SerializeField] private bool isHeld = false;
    [SerializeField] private bool isThrown = false;
    [SerializeField] private float throwForce = 20f;
    private Transform playerHand;
    private Rigidbody itemRigidbody;
    private Collider itemCollider;

    [Header("Damage Properties")]
    [SerializeField] private int damageAmount = 20;

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

    public int durability = 3;

    private void Start() {
        playerInteract = GameObject.Find("Player").GetComponent<PlayerInteract>();

        itemRigidbody = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
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

            transform.SetParent(playerHand);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

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
        if (isHeld && Input.GetMouseButtonDown(0)) {
            Throw();
        }
    }

    private void Throw() {
        isHeld = false;
        isThrown = true;

        transform.SetParent(null);

        itemRigidbody.isKinematic = false;
        itemRigidbody.useGravity = true;

        itemCollider.enabled = true;
        itemCollider.isTrigger = false;

        Vector3 throwPoint = Helper.CameraCenterTargetPoint();
        Vector3 throwVelocity = (throwPoint - transform.position).normalized * throwForce;
        itemRigidbody.velocity = throwVelocity;
            
        ShowUI = false;
    }

    private void OnCollisionEnter(Collision other) {
        if (!isHeld && isThrown) {
            if (other.collider.CompareTag("Limb")){
                Debug.Log("Limb hit!");
                
                Ragdoll ragdoll = Helper.GetComponentInParentByTag<Ragdoll>(other.transform, "Enemy");
                if (ragdoll != null) {
                    ragdoll.EnableRagdoll();
                }
                
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

            // Decrease durability on collision
            ReduceDurabilityByCollision(reduceByImpactDurability);

            Helper.CameraShake(hurtShakeMagnitude, hurtShakeDuration);
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
        Debug.Log($"Throwable item {gameObject.name} broke!");
        Destroy(gameObject);
    }
}