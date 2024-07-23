using System.Collections;
using UnityEngine;

public class PlayerSwingable : MonoBehaviour 
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
            StartCoroutine(Swing());
        }

        if (Input.GetMouseButton(1)) {
            StartCoroutine(Block(true));
        }
        else if (Input.GetMouseButtonUp(1)) {
            StartCoroutine(Block(false));
        }
    }
    
    private IEnumerator Swing() {
        if(playerInteract.currentHeldItem != null) {
            if(playerInteract.currentHeldItem.TryGetComponent(out Swingable swingable)) {
                swingable.isAttacking = true;

                playerInteract.playerAnimator.SetTrigger("Attack");

                yield return new WaitForSeconds(swingable.weaponData.attackSpeed);

                swingable.isAttacking = false;
            }
        }
    }
    private IEnumerator Block(bool hasBlocked) {
        if(playerInteract.currentHeldItem != null) {
            if(playerInteract.currentHeldItem.TryGetComponent(out Swingable swingable)) {
                swingable.isBlocking = hasBlocked;
                
                playerInteract.playerAnimator.SetBool("Block", swingable.isBlocking);
                
                yield return null;
            }
        }
    }

    public Swingable CurrentHeldItemIsSwingable() {
        if(playerInteract.currentHeldItem != null) {
            if(playerInteract.currentHeldItem.TryGetComponent(out Swingable swingable)) {
                return swingable;
            }
        }
        return null;
    }
}