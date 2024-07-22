using UnityEngine;

public class PlayerAnimations : MonoBehaviour 
{
    private PlayerAttack playerAttack;
    private PlayerInteract playerInteract;
    private PlayerMovementRigidbody playerMovementRigidbody;
    private string currentAnimationState;

    [SerializeField] private Animator playerAnimator;

    public const string IDLE = "Idle";

    private void Start() {
        if (playerAnimator == null) {
            playerAnimator = GetComponentInChildren<Animator>();
        }
        if (playerAttack == null) {
            playerAttack = GetComponent<PlayerAttack>();
        }
        if (playerInteract == null) {
            playerInteract = GetComponent<PlayerInteract>();
        }
        if (playerMovementRigidbody == null) {
            playerMovementRigidbody = GetComponent<PlayerMovementRigidbody>();
        }
    }
    // private void Update() {
    //     SetAnimations();
    // }

    // public void SetAnimations() {
    //     if(playerInteract.currentHeldItem == null) {
    //         if(!playerAttack.isAttacking) {
    //             if (playerMovementRigidbody.isWalking) {
    //                 ChangeAnimationState(IDLE, 0.6f);
    //             }
    //             else if (playerMovementRigidbody.isRunning) {
    //                 ChangeAnimationState(IDLE, 1.0f);
    //             }
    //             else if (playerMovementRigidbody.isCrouching) {
    //                 ChangeAnimationState(IDLE, 0.1f);
    //             }
    //             else {
    //                 ChangeAnimationState(IDLE, 0.3f);
    //             }
    //         }
    //     }
    //     else {
    //         //
    //     }
    // }
    public void ChangeAnimationState(string newState = IDLE, float animSpeed = 1f) {
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState && playerAnimator.speed == animSpeed) {
            return;
        }

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        playerAnimator.speed = animSpeed;
        playerAnimator.CrossFadeInFixedTime(currentAnimationState, 0.1f);
    }
}