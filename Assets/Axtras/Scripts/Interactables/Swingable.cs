using UnityEngine;

public class Swingable : Pickable
{
    public bool isAttacking = false;
    public bool isBlocking = false;

    [Header("Swingable Properties")]
    public MeleeItemData itemData;

    public override void Pickup() {
        if (!isHeld) {
            isHeld = true;
            ShowUI = false;
            
            transform.SetParent(playerInteract.playerInteractHolder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;

            playerAnimations.ChangeAnimationState(itemData.idleAnimName);

            playerInteract.currentHeldItemType = PickableType.Swingable;
            playerInteract.pickupIconImage.sprite = itemData.icon;
        }                   
        else {
            Debug.Log($"Item {gameObject.name} is already held");
        }
    }
    public override void Drop(bool destroyItem) {
        if (isHeld) {
            isHeld = false;
            ShowUI = false;

            transform.SetParent(null);
            
            itemRigidbody.isKinematic = false;
            itemRigidbody.useGravity = true;

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;

            playerAnimations.ChangeAnimationState();

            playerInteract.currentHeldItem = null;
            playerInteract.currentHeldItemType = PickableType.None;
            playerInteract.pickupIconImage.sprite = playerAction.meleeItemData.icon;
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }
}
