using UnityEngine;
using UnityEngine.UI;

public class Consumable : MonoBehaviour 
{
    private PlayerHealth playerHealth;
    private PlayerPickups playerPickups;
    private bool isConsuming = false;
    private Image consumableMeterImage;

    [Header("Consumable Pickup Properties")]
    public float consumptionDuration = 2f;
    private float consumptionProgress = 0.0f;

    [Header("Consumable Properties")]
    [SerializeField] private int healthAddAmount = 20;

    private void Start() {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerPickups = FindObjectOfType<PlayerPickups>();

        if(consumableMeterImage == null) {
            consumableMeterImage = GameObject.Find("Consumable Meter Cursor Image").GetComponent<Image>();
        }
    }

    private void Update() {
        HandleConsumable();
    }

    // Consumable Related
    private void HandleConsumable() {
        if (playerPickups.currentPlayerPickupItem.type == PlayerPickupItems.PickupType.Consumable) {
            // Main
            if (Input.GetMouseButton(0)) {
                if (!isConsuming) {
                    isConsuming = true;
                    consumptionProgress = 0f;
                }
                ConsumePickup();
            }
            else {
                isConsuming = false;
                consumptionProgress = 0f;
                consumableMeterImage.fillAmount = 0f;
            }
        }
    }
    private void ConsumePickup() {
        consumptionProgress += Time.deltaTime / consumptionDuration;
        consumableMeterImage.fillAmount = consumptionProgress;

        if (consumptionProgress >= 1f) {
            Debug.Log($"Consumption progress 100%");

            // Consume the item
            playerHealth.AddHealth(healthAddAmount);

            // Reset the consumption meters and UI
            isConsuming = false;
            consumptionProgress = 0f;
            consumableMeterImage.fillAmount = consumptionProgress;

            // Reset the current pickup
            playerPickups.DisableAllPickups();
        }
    }
}