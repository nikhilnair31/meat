using UnityEngine;

public class PlayerAnimations : MonoBehaviour 
{
    private PlayerAttack playerAttack;
    private PlayerInteract playerInteract;
    private PlayerMovementRigidbody playerMovementRigidbody;
    private PlayerConsumable playerConsumable;
    private PlayerThrowable playerThrowable;
    private PlayerSwingable playerSwingable;
    private string currentAnimationState;

    public const string IDLE = "Idle";
    public const string WALK = "Walking";
    public const string RUN = "Running";
    public const string CROUCH = "Crouching";

    [SerializeField] private Animator playerAnimator;

    private void Start() {
        playerAttack = GetComponent<PlayerAttack>();
        playerInteract = GetComponent<PlayerInteract>();
        playerMovementRigidbody = GetComponent<PlayerMovementRigidbody>();
        playerConsumable = GetComponent<PlayerConsumable>();
        playerThrowable = GetComponent<PlayerThrowable>();
        playerSwingable = GetComponent<PlayerSwingable>();

        playerAnimator = GetComponentInChildren<Animator>();

        ChangeAnimationState();
    }

    private void Update() {
        if(playerAttack.playerIsUnarmed) {
            if (playerMovementRigidbody.isRunning && !playerMovementRigidbody.isCrouching) {
                ChangeAnimationState(RUN);
            }
            else if (playerMovementRigidbody.isWalking) {
                ChangeAnimationState(WALK);
            }
            else if (playerMovementRigidbody.isCrouching) {
                ChangeAnimationState(CROUCH);
            }
            else {
                ChangeAnimationState(IDLE);
            }
        }
        else {
            Consumable consumable = playerConsumable.CurrentHeldItemIsConsumable();
            Throwable throwable = playerThrowable.CurrentHeldItemIsThrowable();
            Swingable swingable = playerSwingable.CurrentHeldItemIsSwingable();

            if (consumable != null) {
                ChangeAnimationState(consumable.consumableData.holdingAnimationName);
            }
            else if (throwable != null) {
                ChangeAnimationState(throwable.weaponData.holdingAnimationName);
            }
            else if (swingable != null) {
                ChangeAnimationState(swingable.weaponData.holdingAnimationName);
            }
        }
    }

    public void ChangeAnimationState(string newState = "Idle", float animSpeed = 1f) {
        // Debug.Log($"Changing state from {currentAnimationState} to {newState} with speed {animSpeed} while speed is {animSpeed} and player unarmed {playerAttack.playerIsUnarmed}");

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