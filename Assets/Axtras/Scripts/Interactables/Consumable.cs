using UnityEngine;
using UnityEngine.UI;

public class Consumable : PickableLimb
{
    private Image fillableCursorImage;
    private Image pickupIconImage;
    private bool isHeld = false;

    [Header("Consumable Properties")]
    [SerializeField] private ConsumableData consumableData;
    private float holdTime = 0f;

    public override void Interact() {
        Pickup();
    }
    public override void Pickup() {
        if (!isHeld) {
            isHeld = true;
            
            playerHand = playerInteract.playerInteractHolder;
            fillableCursorImage = playerInteract.fillableCursorImage;
            pickupIconImage = playerInteract.pickupIconImage;
            playerInteract.playerAnimator = animator;

            transform.SetParent(playerHand);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = false;
            itemCollider.isTrigger = false;

            animator.enabled = true;

            pickupIconImage.sprite = consumableData.pickupIcon;

            playerAnimations.ChangeAnimationState(consumableData.HOLDING);

            playerAttack.playerIsUnarmed = false;
            
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

            animator.enabled = false;

            pickupIconImage.sprite = null;
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }

    protected override void Update() {
        base.Update();
        
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
            fillableCursorImage.fillAmount = holdTime / consumableData.consumeTime;
            playerMovementRigidbody.isConsuming = true;
            playerMovementRigidbody.speedReductionMultiplier = consumableData.speedReductionMultiplier;

            if (holdTime >= consumableData.consumeTime) {
                Consume();
            }
        }
    }

    void ResetConsumptionOnMouseRelease() {
        holdTime = 0f;
        fillableCursorImage.fillAmount = 0f;
        playerMovementRigidbody.isConsuming = false;
        playerMovementRigidbody.speedReductionMultiplier = 1f;
    }

    void Consume() {
        Debug.Log("Player healed!");
        
        playerHealth.AddHealth(
            consumableData.healAmount, 
            consumableData.healTime
        );
        ResetConsumptionOnMouseRelease();

        Destroy(gameObject);
    }
}