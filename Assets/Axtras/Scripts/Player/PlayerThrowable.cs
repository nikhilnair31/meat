using UnityEngine;

public class PlayerThrowable : MonoBehaviour 
{
    private PlayerMovementRigidbody playerMovementRigidbody;
    private PlayerAnimations playerAnimations;
    private PlayerInteract playerInteract;
    private PlayerHealth playerHealth;

    private void Start() {
        playerMovementRigidbody = GetComponent<PlayerMovementRigidbody>();
        playerAnimations = GetComponent<PlayerAnimations>();
        playerInteract = GetComponent<PlayerInteract>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Throw();
        }
    }

    private void Throw() {
        if(playerInteract.currentHeldItem != null) {
            if(playerInteract.currentHeldItem.TryGetComponent(out Throwable throwable)) {
                throwable.Throw();
                playerAnimations.ChangeAnimationState(throwable.weaponData.throwingAnimationName);
            }
        }
    }
}