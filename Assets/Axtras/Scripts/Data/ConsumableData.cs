using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableData", menuName = "ScriptableObjects/ConsumableData", order = 1)]
public class ConsumableData : ScriptableObject
{
    [Header("Heal Properties")]
    public int healAmount = 20;
    public float healTime = 3f;
    public float consumeTime = 3f;

    [Header("Move Properties")]
    public float speedReductionMultiplier = 0.8f;

    [Header("UI Settings")]
    public Sprite pickupIcon;
}