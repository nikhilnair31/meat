using UnityEngine;

public class Throwable : MonoBehaviour 
{
    private PlayerPickups playerPickups;

    [Header("Throwable Properties")]
    [SerializeField] private float throwForce = 20f;

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
        originalColor = weaponMaterial.color;

        currentHealth = maxHealth;
    }

    private void Update() {
        HandleThrowable();
    }
    // Throwable Related
    private void HandleThrowable() {
        if (playerPickups.currentPlayerPickupItem.type == PlayerPickupItems.PickupType.Throwable) {
            if (Input.GetMouseButtonDown(0)) {
                ThrowPickup();
            }
        }
    }
    private void ThrowPickup() {
        if(playerPickups.currentPlayerPickupItem.prefab == null) {
            Debug.LogWarning($"Current pickup prefab is null, cancelling throw");
            return;
        }

        // Spawn the object
        GameObject spawnedObject = Instantiate(playerPickups.currentPlayerPickupItem.prefab);

        // Set position and rotation
        spawnedObject.transform.SetLocalPositionAndRotation(
            playerPickups.currentPlayerPickupItem.obj.transform.position, 
            playerPickups.currentPlayerPickupItem.obj.transform.rotation
        );

        // Set name to same as prefab (Remove Clone suffix)
        spawnedObject.transform.name = playerPickups.currentPlayerPickupItem.obj.name;

        // If has Rigidbody, apply velocity to throw
        if (spawnedObject.TryGetComponent(out Rigidbody rb)) {
            Vector3 throwPoint = Helper.CameraCenterTargetPoint();
            Vector3 throwVelocity = (throwPoint - transform.position).normalized * throwForce;
            rb.velocity = throwVelocity;
        }

        // Reset the current pickup
        playerPickups.DisableAllPickups();
    }

    private void OnCollisionEnter(Collision other) {
        ThrowableImpacting(other.transform);
    }
    private void ThrowableImpacting(Transform other) {
        // Debug.Log($"Weapon collided with {other.name} of tag {other.tag}");

        if (other.CompareTag("Limb")) {
            TransformCollector transformCollector = Helper.GetComponentInParentByTag<TransformCollector>(other, "Enemy");
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

        ReduceWeaponHealth(limbStrengthDamage);
        Helper.CameraShake(hurtShakeMagnitude, hurtShakeDuration);
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