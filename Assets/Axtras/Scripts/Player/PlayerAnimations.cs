using UnityEngine;

public class PlayerAnimations : MonoBehaviour 
{
    private string currentAnimationState;

    [SerializeField] private Animator playerAnimator;

    private void Start() {
        playerAnimator = GetComponentInChildren<Animator>();

        ChangeAnimationState();
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