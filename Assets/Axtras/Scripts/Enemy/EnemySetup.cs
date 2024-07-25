using UnityEngine;

public class EnemySetup : MonoBehaviour 
{
    [SerializeField] private TransformCollector transformCollector;
    [SerializeField] private bool randomLimbDestroy = true;
    [SerializeField] private float randomLimbDestroyChance = 0.2f;

    private void Start() {
        transformCollector = transform.GetComponent<TransformCollector>();

        if (randomLimbDestroy) {
            RandomLimbDestroy();
        }
    }

    private void RandomLimbDestroy() {
        foreach (TransformData data in transformCollector.transformDataList) {
            if (Random.value < randomLimbDestroyChance) {
                data.destroyed = true;
            }
        }
    }
}