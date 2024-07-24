using System.Collections;
using UnityEngine;

public abstract class Interactable : MonoBehaviour 
{
    internal PlayerMovementRigidbody playerMovementRigidbody;
    internal PlayerAnimations playerAnimations;
    internal PlayerInteract playerInteract;
    internal PlayerHealth playerHealth;
    internal PlayerAttack playerAttack;

    [Header("UI Related")]
    [SerializeField] private UITransitionData uiTransitionData;
    [SerializeField] private Transform interactTransform;
    private bool showUI;

    public virtual void Interact() {}
    public virtual void Pickup() {}
    public virtual void Drop(bool destroyItem = false) {}

    private void Start() {
        if(interactTransform == null) {
            interactTransform = transform.Find("Interact Canvas");
        }

        playerMovementRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementRigidbody>();
        playerAnimations = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimations>();
        playerInteract = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteract>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
    }

    public bool ShowUI {
        get { 
            return showUI; 
        }
        set {
            if (showUI != value) {
                showUI = value;
                StopAllCoroutines();
                if (showUI) {
                    ShowPickupItemUI();
                }
                else {
                    HidePickupItemUI();
                }
            }
        }
    }
    private void ShowPickupItemUI() {
        // Debug.Log("ShowPickupItemUI");

        interactTransform.position = transform.position + new Vector3(0, uiTransitionData.canvasOffsetY, 0);
        Helper.ScaleInAndOutUI(
            interactTransform, 
            uiTransitionData.transitionAmount, 
            uiTransitionData.transitionDuration
        );
    }
    private void HidePickupItemUI() {
        // Debug.Log("HidePickupItemUI");

        Helper.ScaleInAndOutUI(
            interactTransform, 
            0f, 
            uiTransitionData.transitionDuration
        );
    }
    // FIXME: Causes scale pulsing on constant loking at item canvas
    public IEnumerator HidePickupItemUIAfterDelay() {
        // Debug.Log("HidePickupItemUIAfterDelay");

        yield return new WaitForSeconds(uiTransitionData.displayDuration);
        
        ShowUI = false;
    }
}