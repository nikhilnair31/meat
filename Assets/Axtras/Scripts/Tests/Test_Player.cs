using UnityEngine;

public class Test_Player : MonoBehaviour 
{
    [SerializeField] private float attachDistanceRange = 4f;

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, attachDistanceRange);
    }    
}