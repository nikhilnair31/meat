using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Swingable : PickableLimb
{
    public bool isAttacking = false;
    public bool isBlocking = false;

    [Header("Swingable Properties")]
    public SwingableWeaponData weaponData;

    [Header("Effects")]
    [SerializeField] private ImpactEffectData impactEffectData;

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

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;

            playerAnimations.ChangeAnimationState(weaponData.holdingAnimationName);

            playerInteract.pickupIconImage.sprite = weaponData.pickupIcon;

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
