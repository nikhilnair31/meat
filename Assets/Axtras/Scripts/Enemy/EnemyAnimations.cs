using UnityEngine;

public class EnemyAnimations : MonoBehaviour 
{
    private string currentAnimationState;

    [SerializeField] private Animator enemyAnimator;

    private void Start() {
        enemyAnimator = GetComponent<Animator>();

        ChangeAnimationState();
    }

    public void ChangeAnimationState(string newState = "Idle", float animSpeed = 1f) {
        // Debug.Log($"Changing state from {currentAnimationState} to {newState} with speed {animSpeed} while speed is {animSpeed} and player unarmed {playerAttack.playerIsUnarmed}");

        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState && enemyAnimator.speed == animSpeed) {
            return;
        }

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        enemyAnimator.speed = animSpeed;
        enemyAnimator.CrossFadeInFixedTime(currentAnimationState, 0.1f);
    }
}