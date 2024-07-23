using UnityEngine;

public class PlayerThrowable : MonoBehaviour 
{
    private PlayerAnimations playerAnimations;
    private PlayerInteract playerInteract;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Throw();
        }
    }

    private void Throw() {
        // if(playerInteract.currentHeldItem.gameobject.TryGetComponent(out Throwable throwable)) {
            
        // }
    }
}