using UnityEngine;
using UnityEngine.UI;

public class Consumable : PickableLimb
{
    private Image fillableCursorImage;

    [Header("Main")]
    [SerializeField] private bool isHeld = false;

    [Header("Heal Properties")]
    [SerializeField] public int healAmount = 20;
    [SerializeField] public float healTime = 3f;
    [SerializeField] public float consumeTime = 3f;
    [SerializeField] private float holdTime = 0f;

    public override void Interact() {
        Pickup();
    }
    public override void Pickup() {
        if (!isHeld) {
            isHeld = true;
            
            playerHand = playerInteract.playerInteractHolder;
            fillableCursorImage = playerInteract.fillableCursorImage;
            playerInteract.playerAnimator = animator;

            transform.SetParent(playerHand);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            animator.enabled = true;

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = false;
            itemCollider.isTrigger = false;
            
            ShowUI = false;
        }
        else {
            Debug.Log($"Item {gameObject.name} is already held");
        }
    }
    public override void Drop() {
        if (isHeld) {
            isHeld = false;

            transform.SetParent(null);
            
            itemRigidbody.isKinematic = false;
            itemRigidbody.useGravity = true;

            itemCollider.enabled = true;
            itemCollider.isTrigger = false;
        }
        else {
            Debug.Log($"Item {gameObject.name} NOT held");
        }
    }

    private void Update() {
        if (isHeld) {
            if(Input.GetMouseButton(0)) {
                HandleConsumption();
            }
            else if(Input.GetMouseButtonUp(0)) {
                ResetConsumptionOnMouseRelease();
            }
        }
    }

    void HandleConsumption() {
        if (Input.GetMouseButton(0)) {
            holdTime += Time.deltaTime;
            fillableCursorImage.fillAmount = holdTime / consumeTime;

            if (holdTime >= consumeTime) {
                Consume();
            }
        }
    }

    void ResetConsumptionOnMouseRelease() {
        holdTime = 0f;
        fillableCursorImage.fillAmount = 0f;
    }

    void Consume() {
        Debug.Log("Player healed!");
        
        playerHealth.AddHealth(healAmount, healTime);
        ResetConsumptionOnMouseRelease();

        Destroy(gameObject);
    }
}