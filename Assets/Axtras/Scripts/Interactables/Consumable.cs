using UnityEngine;

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

            transform.SetParent(playerInteract.playerInteractHolder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = false;
            itemCollider.isTrigger = false;

            playerAnimations.ChangeAnimationState(consumableData.holdingAnimationName);

            playerInteract.pickupIconImage.sprite = consumableData.pickupIcon;

            playerAttack.playerIsUnarmed = false;
        }
        else {
            Debug.Log($"Item {gameObject.name} is already held");
        }
    }
    public override void Drop() {
        if (isHeld) {
            isHeld = false;
            ShowUI = false;

            transform.SetParent(null);
            
            itemRigidbody.isKinematic = false;
            itemRigidbody.useGravity = true;

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;

            playerInteract.currentHeldItem = null;
            playerInteract.pickupIconImage.sprite = playerAttack.meleeWeaponData.weaponIcon;

            playerAttack.playerIsUnarmed = true;
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }
}