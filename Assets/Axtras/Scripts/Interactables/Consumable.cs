using UnityEngine;

public class Consumable : Pickables
{
    [Header("Consumable Properties")]
    public ConsumableItemData itemData;

    public override void Interact() {
        Pickup();
    }
    public override void Pickup() {
        if (!isHeld) {
            isHeld = true;
            ShowUI = false;

            transform.SetParent(playerInteract.playerInteractHolder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = false;
            itemCollider.isTrigger = false;

            playerAnimations.ChangeAnimationState(itemData.holdingAnimationName);

            playerInteract.pickupIconImage.sprite = itemData.pickupIcon;

            playerAttack.playerIsUnarmed = false;
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
            playerInteract.pickupIconImage.sprite = playerAttack.meleeWeaponData.weaponIcon;

            playerAttack.playerIsUnarmed = true;

            if (destroyItem) {
                Destroy(this.gameObject);
            }
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }
}