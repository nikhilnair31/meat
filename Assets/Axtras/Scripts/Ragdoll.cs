using UnityEngine;

public class Ragdoll : MonoBehaviour 
{
    public bool isRagdoll = false;

    private void Start() {
        DisableRagdoll();
    }

    private void DisableRagdoll() {
        isRagdoll = false;
        GetComponent<Animator>().enabled = true;
        DisableAllPhysics();
    }
    private void DisableAllPhysics() {
        foreach (Rigidbody rgb in GetComponentsInChildren<Rigidbody>()) {
            rgb.isKinematic = true;
            rgb.useGravity = false;
        }
        foreach (Collider coll in GetComponentsInChildren<Collider>()) {
            coll.enabled = true;
            coll.isTrigger = false;
        }
    }

    public void EnableRagdoll() {
        isRagdoll = true;
        GetComponent<Animator>().enabled = false;
        EnableAllPhysics();
    }
    private void EnableAllPhysics() {
        foreach (Rigidbody rgb in GetComponentsInChildren<Rigidbody>()) {
            rgb.isKinematic = false;
            rgb.useGravity = true;
        }
        foreach (Collider coll in GetComponentsInChildren<Collider>()) {
            coll.enabled = true;
            coll.isTrigger = false;
        }
    }
}