using UnityEngine;

public class Consumable : Pickable
{
    [Header("Consumable Properties")]
    public ConsumableItemData itemData;

    public override void Pickup() {
        ShowUI = false;

        if (!isHeld) {
            isHeld = true;

            transform.SetParent(playerInteract.playerInteractHolder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = false;
            itemCollider.isTrigger = false;

            playerAnimations.ChangeAnimationState(itemData.holdingAnimationName);

            playerInteract.currentHeldItemType = PickableType.Consumable;
            playerInteract.pickupIconImage.sprite = itemData.icon;
        }
        else {
            Debug.Log($"Item {gameObject.name} is already held");
        }
    }
    public override void Drop(bool destroyItem) {
        ShowUI = false;

        if (isHeld) {
            isHeld = false;

            transform.SetParent(null);
            
            itemRigidbody.isKinematic = false;
            itemRigidbody.useGravity = true;

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;

            playerAnimations.ChangeAnimationState();

            playerInteract.currentHeldItem = null;
            playerInteract.currentHeldItemType = PickableType.None;
            playerInteract.pickupIconImage.sprite = playerAction.meleeItemData.icon;

            if (destroyItem) {
                Destroy(gameObject);
            }
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }
}