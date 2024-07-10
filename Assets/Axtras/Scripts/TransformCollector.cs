using UnityEngine;
using System.Collections.Generic;

public class TransformCollector : MonoBehaviour
{
    [Header("Main Settings")]
    public List<TransformData> transformDataList = new();
    
    [Header("Deafult Settings")]
    [SerializeField] private GameObject defaultSpawnedLimbObject;
    [SerializeField] private GameObject defaultDamageImpactObject;

    private void Start() {
        foreach (TransformData data in transformDataList)
        {
            data.Init(defaultSpawnedLimbObject, defaultDamageImpactObject);
        }
    }
    private void Update()
    {
        foreach (TransformData data in transformDataList)
        {
            data.UpdateScale();
        }
    }
}