using System.Collections;
using UnityEngine;

public abstract class Interactable : MonoBehaviour 
{
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
        Debug.Log("ShowPickupItemUI");
        interactTransform.position = transform.position + new Vector3(0, uiTransitionData.canvasOffsetY, 0);
        Helper.ScaleInAndOutUI(
            interactTransform, 
            uiTransitionData.transitionAmount, 
            uiTransitionData.transitionDuration
        );
    }
    private void HidePickupItemUI() {
        Debug.Log("HidePickupItemUI");
        Helper.ScaleInAndOutUI(
            interactTransform, 
            0f, 
            uiTransitionData.transitionDuration
        );
    }
    // FIXME: Causes scale pulsing on constant loking at item canvas
    public IEnumerator HidePickupItemUIAfterDelay() {
        Debug.Log("HidePickupItemUIAfterDelay");

        yield return new WaitForSeconds(uiTransitionData.displayDuration);
        
        ShowUI = false;
    }
}