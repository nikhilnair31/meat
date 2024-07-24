using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableItemData", menuName = "ScriptableObjects/ConsumableItemData", order = 1)]
public class ConsumableItemData : ScriptableObject
{
    [Header("Heal Properties")]
    public int healAmount = 20;
    public float healTime = 3f;
    public float consumeTime = 3f;

    [Header("Move Properties")]
    public float speedReductionMultiplier = 0.8f;

    [Header("UI Settings")]
    public Sprite icon;

    [Header("Animation Settings")]
    public string holdingAnimationName;
}