using System.Collections.Generic;
using UnityEngine;

public class PlayerPickups : MonoBehaviour 
{
    public Dictionary<string, PlayerPickupItems.PickupType> pickupTypeMap = new()
    {
        { "Swingable", PlayerPickupItems.PickupType.Swingable },
        { "Throwable", PlayerPickupItems.PickupType.Throwable },
        { "Consumable", PlayerPickupItems.PickupType.Consumable }
    };
    public PlayerPickupItems currentPlayerPickupItem = null;
    public GameObject currentPlayerPickupItemGameObject;
    public GameObject currentPlayerPickupItemPrefab;
    public PlayerPickupItems.PickupType currentPlayerPickupItemPickupType;

    [Header("Pickup Properties")]
    [SerializeField] private Transform playerEyes;
    public Transform pickupHolder;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private float pickupRange = 2f;
    public List<PlayerPickupItems> playerPickupItemsList;

    [Header("UI Elements")]
    [SerializeField] private RectTransform cursorImageRectTransform;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 10f;
    
    private void Start() {
    }

    private void Update() {
        TryPickingUpItem();
        LookingAtItems();
    }

    private void LookingAtItems() {
        if (Physics.Raycast(playerEyes.position, playerEyes.forward, out RaycastHit hit, pickupRange, pickupLayer)) {
            if (hit.collider.CompareTag("Pickup")) {
                hit.collider.gameObject.GetComponent<Pickups>().ShowUI = true;
            }
        }
    }

    private void TryPickingUpItem() {
        if (Input.GetKeyDown(KeyCode.E)) {
            LookForItem();
        }
    }
    private void LookForItem() {
        // Debug.Log($"LookForItem");

        if (Physics.Raycast(playerEyes.position, playerEyes.forward, out RaycastHit hit, pickupRange, pickupLayer)) {
            // Debug.Log($"item near enough: {hit.collider.tag}");
            
            if (hit.collider.CompareTag("Pickup")) {
                CheckPickup(hit.collider.gameObject);
            }
        }
        else {
            Debug.Log("item too far!");
            Helper.UIShake(cursorImageRectTransform, shakeMagnitude, shakeDuration);
        }
    }
    // TODO: Reset picked limb's health
    // TODO: Display picked limb's health
    private void CheckPickup(GameObject hitGameObject) {
        // Debug.Log($"CheckPickup");

        string pickupName = hitGameObject.name;
        // Debug.Log($"pickupName: {pickupName}");

        PlayerPickupItems pickup = FindPickupByName(pickupName);
        // Debug.Log($"has pickup: {pickup != null}");

        if (pickup != null) {
            EquipPickup(pickup);

            hitGameObject.SetActive(false);
        }
        else {
            Debug.Log($"Pickup not allowed: {pickupName}");
            Helper.UIShake(cursorImageRectTransform, shakeMagnitude, shakeDuration);
        }
    }
    private void EquipPickup(PlayerPickupItems pickup) {
        // Debug.Log($"EquipPickup | currentPlayerPickupItem: {currentPlayerPickupItem} | pickup: {pickup}");

        // If player's currently holding something then turn that off
        DisableAllPickups();
        EnableOnePickup(pickup);

    }
    public void DisableAllPickups() {
        foreach (Transform child in pickupHolder) {
            child.gameObject.SetActive(false);
        }
        
        if(currentPlayerPickupItemGameObject != null) {
            currentPlayerPickupItemGameObject.SetActive(false);
        }
        currentPlayerPickupItemGameObject = null;
        currentPlayerPickupItem = null;
    }
    public void EnableOnePickup(PlayerPickupItems pickup) {
        currentPlayerPickupItem = pickup;
        currentPlayerPickupItemPickupType = pickup.type;
        currentPlayerPickupItemGameObject = pickup.obj;
        currentPlayerPickupItemPrefab = pickup.prefab;
        currentPlayerPickupItemGameObject.SetActive(true);
    }
    
    private PlayerPickupItems FindPickupByName(string itemName) {
        foreach (var item in playerPickupItemsList) {
            if (item.name == itemName) {
                return item;
            }
        }
        return null;
    }
}