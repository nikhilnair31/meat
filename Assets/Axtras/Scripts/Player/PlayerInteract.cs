using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour 
{
    private Interactable lastSeenInteractable;
    private Interactable currentHeldItem;

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

    void Update() {
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
    }
    private void ItemInteractRaycast() {
        if (lastSeenInteractable != null) {
            lastSeenInteractable.Interact();
        }
        else {
            if (Physics.Raycast(playerEyes.position, playerEyes.forward, out RaycastHit hit, interactableRange, interactableLayer)) {
                if (hit.collider.TryGetComponent<Interactable>(out Interactable interactable)) {
                    Debug.Log($"Interact with {hit.collider.name}");
                    if (currentHeldItem != null) {
                        currentHeldItem.Drop();
                    }
                    currentHeldItem = interactable;
                    interactable.Interact();
                }
            }
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