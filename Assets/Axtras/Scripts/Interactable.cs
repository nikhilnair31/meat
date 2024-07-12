using System.Collections;
using UnityEngine;

public abstract class Interactable : MonoBehaviour 
{
    private bool showUI;

    [Header("UI Related")]
    [SerializeField] private Transform interactTransform;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float transitionAmount = 0.3f;
    [SerializeField] private float canvasOffsetY = 3f;
    [SerializeField] private float displayDuration = 2f;

    public virtual void Interact() {}
    public virtual void Pickup() {}
    public virtual void Drop() {}

    public bool ShowUI {
        get { 
            return showUI; 
        }
        set {
            if (showUI != value) {
                showUI = value;
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
        // Debug.Log($"ShowPickupItemUI");

        if(interactTransform != null) {
            interactTransform.position = transform.position + new Vector3(0, canvasOffsetY, 0);
            Helper.ScaleInAndOutUI(interactTransform, transitionAmount, transitionDuration);
            StartCoroutine(HidePickupItemUIAfterDelay());
        }
    }
    private void HidePickupItemUI() {
        // Debug.Log($"HidePickupItemUI");

        if(interactTransform != null) {
            Helper.ScaleInAndOutUI(interactTransform, 0f, transitionDuration);
        }
    }
    // FIXME: Causes scale pulsing on constant loking at item canvas
    private IEnumerator HidePickupItemUIAfterDelay() {
        // Debug.Log($"HidePickupItemUIAfterDelay");
        
        yield return new WaitForSeconds(displayDuration);
        
        // Debug.Log($"done waiting");
        ShowUI = false;
    }
}