using UnityEngine;

public class PlayerThrowable : MonoBehaviour 
{
    private PlayerAnimations playerAnimations;
    private PlayerInteract playerInteract;

    private void Start() {
        playerAnimations = GetComponent<PlayerAnimations>();
        playerInteract = GetComponent<PlayerInteract>();
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

    public Throwable CurrentHeldItemIsThrowable() {
        if(playerInteract.currentHeldItem != null) {
            if(playerInteract.currentHeldItem.TryGetComponent(out Throwable throwable)) {
                return throwable;
            }
        }
        return null;
    }
}