using UnityEngine;

public class Randomizer : MonoBehaviour {
    [Header("Random Settings")]
    [SerializeField] private bool randPos = false;
    [SerializeField] private float randPosAmount = 5f;
    [SerializeField] private bool randRot = false;
    [SerializeField] private float randRotAmount = 360f;

    private void Start() {
        if(randPos) {
            transform.position += new Vector3(
                Random.Range(-randPosAmount, randPosAmount),
                Random.Range(-randPosAmount, randPosAmount),
                Random.Range(-randPosAmount, randPosAmount)
            );
        }
        if(randRot) {
            transform.rotation = Quaternion.Euler(
                Random.Range(-randRotAmount, randRotAmount), 
                Random.Range(-randRotAmount, randRotAmount), 
                Random.Range(-randRotAmount, randRotAmount)
            );
        }
    }
}