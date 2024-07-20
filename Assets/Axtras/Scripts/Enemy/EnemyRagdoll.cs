using UnityEngine;
using UnityEngine.AI;

public class EnemyRagdoll : MonoBehaviour 
{
    [SerializeField] private bool startWithEnemyRagdolled = false;
    private bool isRagdoll = false;
    public bool IsRagdoll {
        get { 
            return isRagdoll; 
        }
        set {
            isRagdoll = value;
            if(isRagdoll) {
                EnableAllPhysics();
            }
            else {
                DisableAllPhysics();
            }
        }
    }

    private void Start() {
        IsRagdoll = startWithEnemyRagdolled;
    }

    public void EnableRagdoll() {
        IsRagdoll = true;
    }
    public void DisableRagdoll() {
        IsRagdoll = false;
    }

    private void DisableAllPhysics() {
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
    private void EnableAllPhysics() {
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
        
        GetComponentInChildren<EnemyMelee>().DisableOnRagdoll();
    }   
}