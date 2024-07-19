using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour 
{
    private Interactable currentHeldItem;

    [Header("Main")]
    public Animator playerAnimator;
    [SerializeField] private Transform playerEyes;
    public Transform playerInteractHolder;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactableRange = 5f;

    [Header("UI Related")]
    public Image baseCursorImage;
    public Image fillableCursorImage;

    void Update() {
        HandleItemViewing();
        HandleItemInteraction();
    }

    private void HandleItemViewing() {
        if (Physics.Raycast(playerEyes.position, playerEyes.forward, out RaycastHit hit, interactableRange, interactableLayer)) {
            if (hit.collider.TryGetComponent<Interactable>(out Interactable interactable)) {
                interactable.ShowUI = true;
            }
        }
    }

    void HandleItemInteraction() {
        if (Input.GetKeyDown(KeyCode.E)) {
            ItemInteractRaycast();
        }
    }
    private void ItemInteractRaycast() {
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