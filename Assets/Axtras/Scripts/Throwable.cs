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

        Init();
    }
    // Init Related
    private void Init() {
        currentHealth = maxHealth;
        originalColor = weaponMaterial.color;
    }

    private void Update() {
        HandleThrowable();
    }
    // Throwable Related
    private void HandleThrowable() {
        if (playerPickups.currentPlayerPickupItemPickupType == PlayerPickupItems.PickupType.Throwable) {
            // Main
            if (Input.GetMouseButtonDown(0)) {
                ThrowPickup();
            }
        }
    }
    private void ThrowPickup() {
        if(playerPickups.currentPlayerPickupItemPrefab == null) {
            Debug.LogWarning($"Current pickup prefab is null, cancelling throw");
            return;
        }

        // Spawn the object
        GameObject spawnedObject = Instantiate(playerPickups.currentPlayerPickupItemPrefab);

        // Set position and rotation
        spawnedObject.transform.SetLocalPositionAndRotation(
            playerPickups.currentPlayerPickupItemGameObject.transform.position, 
            playerPickups.currentPlayerPickupItemGameObject.transform.rotation
        );

        // Set name to same as prefab (Remove Clone suffix)
        spawnedObject.transform.name = playerPickups.currentPlayerPickupItemGameObject.name;

        // If has Rigidbody, apply velocity to throw
        if (spawnedObject.TryGetComponent(out Rigidbody rb)) {
            Vector3 throwVelocity = (ThrowTargetPoint() - transform.position).normalized * throwForce;
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
    
    private Vector3 ThrowTargetPoint() {
        // FIXME: Make thrown item go to center of camera
        Camera mainCamera = Camera.main;
        Vector3 screenCenter = new(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane + 1f);
        Vector3 screenCenterWorldPoint = mainCamera.ScreenToWorldPoint(screenCenter);
        Vector3 throwDirection = (screenCenterWorldPoint - mainCamera.transform.position).normalized;

        // Define the maximum throw distance
        float maxThrowDistance = 100f;

        // Perform a raycast from the camera's position in the throw direction
        Ray ray = new(mainCamera.transform.position, throwDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, maxThrowDistance)) {
            return hit.point;
        }
        else {
            return mainCamera.transform.position + throwDirection * maxThrowDistance;
        }
    }   
}