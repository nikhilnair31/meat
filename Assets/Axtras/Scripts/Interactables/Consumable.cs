using UnityEngine;
using UnityEngine.UI;

public class Consumable : PickableLimb
{
    [Header("Consumable Properties")]
    public ConsumableData consumableData;

    public override void Interact() {
        Pickup();
    }
    public override void Pickup() {
        if (!isHeld) {
            isHeld = true;
            ShowUI = false;
            
            playerHand = playerInteract.playerInteractHolder;
            playerInteract.playerAnimator = animator;

            transform.SetParent(playerHand);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = false;
            itemCollider.isTrigger = false;

            // animator.enabled = true;

            playerInteract.pickupIconImage.sprite = consumableData.pickupIcon;

            playerAnimations.ChangeAnimationState(consumableData.holdingAnimationName);

            playerAttack.playerIsUnarmed = false;
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

            // animator.enabled = false;

            playerInteract.pickupIconImage.sprite = null;

            playerAnimations.ChangeAnimationState();

            playerAttack.playerIsUnarmed = true;
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }
}