using UnityEngine;

public class PlayerConsumable : MonoBehaviour 
{
    private PlayerInteract playerInteract;
    private PlayerHealth playerHealth;
    private float holdTime = 0f;

    [Header("Consuming Settings")]
    public bool isConsuming = false;
    public float speedReductionMultiplier = 1f;

    private void Start() {
        playerInteract = GetComponent<PlayerInteract>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update() {
        if(Input.GetMouseButton(0)) {
            HandleConsumption();
        }
        else if(Input.GetMouseButtonUp(0)) {
            ResetConsumptionOnMouseRelease();
        }
    }

    private void HandleConsumption() {
        Consumable consumable = CurrentHeldItemIsConsumable();
        if(consumable != null) {
            holdTime += Time.deltaTime;
            playerInteract.fillableCursorImage.fillAmount = holdTime / consumable.itemData.consumeTime;
            speedReductionMultiplier = consumable.itemData.speedReductionMultiplier;
            isConsuming = true;

            if (holdTime >= consumable.itemData.consumeTime) {
                Debug.Log("Player healed!");
                
                playerHealth.AddHealth(
                    consumable.itemData.healAmount, 
                    consumable.itemData.healTime
                );
                ResetConsumptionOnMouseRelease();

                consumable.Drop(false);
            }
        }
    }
    private void ResetConsumptionOnMouseRelease() {
        holdTime = 0f;
        playerInteract.fillableCursorImage.fillAmount = 0f;
        speedReductionMultiplier = 1f;
        isConsuming = false;
    }
    
    public Consumable CurrentHeldItemIsConsumable() {
        if(playerInteract.currentHeldItem != null) {
            if(playerInteract.currentHeldItem.TryGetComponent(out Consumable consumable)) {
                return consumable;
            }
        }
        return null;
    }
}