using UnityEngine;

[CreateAssetMenu(fileName = "SwingableItemData", menuName = "ScriptableObjects/SwingableItemData", order = 1)]
public class SwingableItemData : ScriptableObject
{
    [Header("Damage Settings")]
    public int damageAmount;
    public float damageDuration;

    [Header("Attack Settings")]
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    public float attackRange = 1f;
    public LayerMask attackLayer;

    [Header("UI Settings")]
    public Sprite pickupIcon;

    [Header("Animation Settings")]
    public string holdingAnimationName;
}