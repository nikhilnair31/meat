using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour 
{
    private Interactable lastSeenInteractable;

    internal Pickable.PickableType currentHeldItemType = Pickable.PickableType.None;
    internal Interactable currentHeldItem;

    [Header("Main")]
    public Animator playerAnimator;
    [SerializeField] private Transform playerEyes;
    public Transform playerInteractHolder;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactableRange = 5f;
    [SerializeField] private float interactablePickupInTime = 1f;
    private float interactablePickupTimer = 0f;

    [Header("UI Related")]
    public Image baseCursorImage;
    public Image fillableCursorImage;
    public Image pickupIconImage;

    private void Update() {
        HandleItemViewing();
        HandleItemInteraction();
        HandleItemPickupTimer();
    }

    private void HandleItemViewing() {
        if (Physics.Raycast(playerEyes.position, playerEyes.forward, out RaycastHit hit, interactableRange, interactableLayer)) {
            if (hit.collider.TryGetComponent<Interactable>(out Interactable interactable)) {
                if (lastSeenInteractable != null && lastSeenInteractable != interactable) {
                    StartCoroutine(lastSeenInteractable.HidePickupItemUIAfterDelay());
                }

                lastSeenInteractable = interactable;
                lastSeenInteractable.ShowUI = true;
                interactablePickupTimer = 0f;
            }
            else {
                StartCoroutine(lastSeenInteractable.HidePickupItemUIAfterDelay());
            }
        }
    }

    void HandleItemInteraction() {
        if (Input.GetKeyDown(KeyCode.E)) {
            ItemInteractRaycast();
        }
        if (Input.GetKeyDown(KeyCode.G)) {
            ItemHeldDrop();
        }
    }
    private void ItemInteractRaycast() {
        if (Physics.Raycast(playerEyes.position, playerEyes.forward, out RaycastHit hit, interactableRange, interactableLayer)) {
            if (hit.collider.TryGetComponent<Pickable>(out Pickable pickable)) {
                Debug.Log($"{hit.collider.name} has Pickable script");
                if (currentHeldItem != null) {
                    currentHeldItem.Drop();
                }
                currentHeldItem = pickable;
                pickable.Pickup();
            }
            else if (hit.collider.TryGetComponent<Interactable>(out Interactable interactable)) {
                Debug.Log($"{hit.collider.name} has Interact script");
                interactable.Interact();
            }
            else {
                Debug.Log($"{hit.collider.name} doesn't have Pickable or Interact script");
            }
        }
    }
    private void ItemHeldDrop() {
        if (currentHeldItem != null) {
            currentHeldItem.Drop();
        }
    }

    private void HandleItemPickupTimer() {
        if (lastSeenInteractable != null) {
            interactablePickupTimer += Time.deltaTime;
            if (interactablePickupTimer >= interactablePickupInTime) {
                lastSeenInteractable = null;
                interactablePickupInTime = 0f;
            }
        }
    }
}