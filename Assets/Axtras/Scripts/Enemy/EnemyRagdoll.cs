using UnityEngine;
using UnityEngine.AI;

public class EnemyRagdoll : MonoBehaviour 
{
    [SerializeField] private bool startWithEnemyRagdolled = false;
    public bool isRagdoll = false;

    private void Start() {
        isRagdoll = startWithEnemyRagdolled;
        if (isRagdoll) {
            EnableRagdoll();
        }
        else {
            DisableRagdoll();
        }
    }

    public void EnableRagdoll() {
        isRagdoll = true;

        GetComponent<Animator>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;

        foreach (Rigidbody rgb in GetComponentsInChildren<Rigidbody>()) {
            rgb.isKinematic = false;
            rgb.useGravity = true;
        }
        foreach (Collider coll in GetComponentsInChildren<Collider>()) {
            coll.enabled = true;
            coll.isTrigger = false;
        }
        
        GetComponentInChildren<EnemyMelee>().DisablePhysicsOnRagdoll();
    }
    public void DisableRagdoll() {
        isRagdoll = false;
        
        GetComponent<Animator>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;

        foreach (Rigidbody rgb in GetComponentsInChildren<Rigidbody>()) {
            rgb.isKinematic = true;
            rgb.useGravity = false;
        }
        foreach (Collider coll in GetComponentsInChildren<Collider>()) {
            coll.enabled = true;
            coll.isTrigger = false;
        }
    }  
}