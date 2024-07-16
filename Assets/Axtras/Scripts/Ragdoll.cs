using UnityEngine;

public class Ragdoll : MonoBehaviour 
{
    [SerializeField] private bool initRagdollOn = false;
    private bool isRagdoll = false;
    public bool IsRagdoll {
        get { 
            return isRagdoll; 
        }
        set {
            if (isRagdoll != value) {
                isRagdoll = value;
                if(isRagdoll) {
                    DisableAllPhysics();
                }
                else {
                    EnableAllPhysics();
                }
            }
        }
    }

    private void Update() {
        IsRagdoll = initRagdollOn;
    }

    public void EnableRagdoll() {
        IsRagdoll = true;
    }
    public void DisableRagdoll() {
        IsRagdoll = false;
    }

    private void DisableAllPhysics() {
        GetComponent<Animator>().enabled = true;
        foreach (Rigidbody rgb in GetComponentsInChildren<Rigidbody>()) {
            rgb.isKinematic = true;
            rgb.useGravity = false;
        }
        foreach (Collider coll in GetComponentsInChildren<Collider>()) {
            coll.enabled = true;
            coll.isTrigger = false;
        }
    }
    private void EnableAllPhysics() {
        GetComponent<Animator>().enabled = false;
        foreach (Rigidbody rgb in GetComponentsInChildren<Rigidbody>()) {
            rgb.isKinematic = false;
            rgb.useGravity = true;
        }
        foreach (Collider coll in GetComponentsInChildren<Collider>()) {
            coll.enabled = true;
            coll.isTrigger = false;
        }
        GetComponent<Enemy>().DisableKickWeapon();
    }
}