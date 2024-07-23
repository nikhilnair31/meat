using UnityEngine;

public class PlayerSwingable : MonoBehaviour 
{
    private PlayerAnimations playerAnimations;
    private PlayerInteract playerInteract;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Swing();
        }
    }

    private void Swing() {
        // if(playerInteract.currentHeldItem.gameobject.TryGetComponent(out Swingable swingable)) {
            
        // }
    }
}