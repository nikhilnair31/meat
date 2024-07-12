using UnityEngine;
using UnityEngine.UI;

public class Consumable : Interactable
{
    private PlayerInteract playerInteract;
    private PlayerHealth playerHealth;
    private Image fillableCursorImage;

    [Header("Main")]
    [SerializeField] private bool isHeld = false;
    private Transform playerHand;
    private Collider itemCollider;
    private Rigidbody itemRigidbody;

    [Header("Heal Properties")]
    [SerializeField] public int healAmount = 20;
    [SerializeField] public float consumeTime = 3f;
    [SerializeField] private float holdTime = 0f;

    void Start() {
        playerInteract = GameObject.Find("Player").GetComponent<PlayerInteract>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        itemCollider = GetComponent<Collider>();
        itemRigidbody = GetComponent<Rigidbody>();
    }

    public override void Interact() {
        Pickup();
    }
    public override void Pickup() {
        if (!isHeld) {
            isHeld = true;
            
            playerHand = playerInteract.playerInteractHolder;
            fillableCursorImage = playerInteract.fillableCursorImage;

            transform.SetParent(playerHand);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;

            itemCollider.enabled = false;
            itemCollider.isTrigger = false;
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
        if (isHeld && Input.GetMouseButton(0)) {
            HandleConsumption();
        }
        if (Input.GetMouseButtonUp(0)) {
            ResetConsumptionOnMouseRelease();
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
        
        playerHealth.AddHealth(healAmount);
        ResetConsumptionOnMouseRelease();

        Destroy(gameObject);
    }
}