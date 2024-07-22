using UnityEngine;

[CreateAssetMenu(fileName = "DurabilityData", menuName = "ScriptableObjects/DurabilityData", order = 1)]
public class DurabilityData : ScriptableObject
{
    [Header("Durability Related")]
    public float reduceByImpactDurability = 2f;
    public float durabilityDecayInTime = 5f;
    public float maxDurability = 20;
}