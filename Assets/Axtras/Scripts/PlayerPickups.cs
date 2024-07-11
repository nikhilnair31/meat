using System.Collections.Generic;
using UnityEngine;

public class PlayerPickups : MonoBehaviour 
{
    public PlayerPickupItems currentPlayerPickupItem = null;

    [Header("Pickup Properties")]
    [SerializeField] private Transform playerEyes;
    public Transform pickupHolder;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactableRange = 2f;
    public List<PlayerPickupItems> playerPickupItemsList;

    [Header("UI Elements")]
    [SerializeField] private RectTransform cursorImageRectTransform;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 10f;
    
    private void Start() {
    }

    private void Update() {
        LookingAtItems();
        TryPickingUpItem();
    }

    private void LookingAtItems() {
        if (Physics.Raycast(playerEyes.position, playerEyes.forward, out RaycastHit hit, interactableRange, interactableLayer)) {
            hit.collider.gameObject.GetComponent<Interactable>().ShowUI = true;
        }
    }

    private void TryPickingUpItem() {
        if (Input.GetKeyDown(KeyCode.E)) {
            InteractWithItem();
        }
    }
    private void InteractWithItem() {
        if (Physics.Raycast(playerEyes.position, playerEyes.forward, out RaycastHit hit, interactableRange, interactableLayer)) {
            // Debug.Log($"item near enough: {hit.collider.tag}");
            
            if (hit.collider.CompareTag("Fireplace")) {
                InteractWithFireplace(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Throwable")) {
                InteractWithThrowable(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Consumable")) {
                InteractWithConsumable(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Swingable")) {
                InteractWithSwingable(hit.collider.gameObject);
            } 
            else {
                Debug.Log($"Pickup of name {hit.collider.name} not allowed");
                Helper.UIShake(cursorImageRectTransform, shakeMagnitude, shakeDuration);
            }
            // TODO: Reset picked limb's health
            // TODO: Display picked limb's health
        }
        else {
            Debug.Log("item too far!");
            Helper.UIShake(cursorImageRectTransform, shakeMagnitude, shakeDuration);
        }
    }
    private void InteractWithFireplace(GameObject hitGameObject) {
        // Debug.Log($"InteractWithFireplace");

        Fireplace fireplace = hitGameObject.GetComponent<Fireplace>();
        fireplace.StartStopFireplace();
    }
    private void InteractWithThrowable(GameObject hitGameObject) {
        Debug.Log($"InteractWithThrowable");
        
        // If player's currently holding something then turn that off
        DisableAllPickups();

        // Enable the object with the player
        EnableOnePickup(
            FindPickupByName(hitGameObject.name)
        );

        // Disable the object to be picked up in the world
        hitGameObject.SetActive(false);
    }
    private void InteractWithConsumable(GameObject hitGameObject) {
        Debug.Log($"InteractWithConsumable");
    }
    private void InteractWithSwingable(GameObject hitGameObject) {
        Debug.Log($"InteractWithSwingable");
    }
    
    public void DisableAllPickups() {
        Debug.Log($"DisableAllPickups");
        
        if (currentPlayerPickupItem != null) {
            // currentPlayerPickupItem.PrintDetails();

            if (currentPlayerPickupItem.obj != null) {
                currentPlayerPickupItem.obj.SetActive(false);
            }
            currentPlayerPickupItem = null;
        }
    }
    public void EnableOnePickup(PlayerPickupItems pickup) {
        currentPlayerPickupItem = pickup;
        currentPlayerPickupItem?.obj.SetActive(true);
    }
    
    public PlayerPickupItems FindPickupByName(string itemName) {
        foreach (var item in playerPickupItemsList) {
            if (item.name == itemName) {
                return item;
            }
        }
        return null;
    }
}