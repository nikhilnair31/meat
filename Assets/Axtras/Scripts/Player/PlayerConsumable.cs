using UnityEngine;

public class PlayerConsumable : MonoBehaviour 
{
    private PlayerMovementRigidbody playerMovementRigidbody;
    private PlayerAnimations playerAnimations;
    private PlayerInteract playerInteract;
    private PlayerHealth playerHealth;
    private float holdTime = 0f;

    private void Start() {
        playerMovementRigidbody = GetComponent<PlayerMovementRigidbody>();
        playerAnimations = GetComponent<PlayerAnimations>();
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
        if(playerInteract.currentHeldItem != null) {
            if(playerInteract.currentHeldItem.TryGetComponent(out Consumable consumable)) {
                holdTime += Time.deltaTime;
                playerInteract.fillableCursorImage.fillAmount = holdTime / consumable.consumableData.consumeTime;
                playerMovementRigidbody.isConsuming = true;
                playerMovementRigidbody.speedReductionMultiplier = consumable.consumableData.speedReductionMultiplier;

                if (holdTime >= consumable.consumableData.consumeTime) {
                    Debug.Log("Player healed!");
                    
                    playerHealth.AddHealth(
                        consumable.consumableData.healAmount, 
                        consumable.consumableData.healTime
                    );
                    ResetConsumptionOnMouseRelease();

                    consumable.Drop();

                    Destroy(playerInteract.currentHeldItem.transform.gameObject);
                }
            }
        }
    }
    private void ResetConsumptionOnMouseRelease() {
        holdTime = 0f;
        playerInteract.fillableCursorImage.fillAmount = 0f;
        playerMovementRigidbody.isConsuming = false;
        playerMovementRigidbody.speedReductionMultiplier = 1f;
    }
}