using UnityEngine;

[CreateAssetMenu(fileName = "MeleeItemData", menuName = "ScriptableObjects/MeleeItemData", order = 1)]
public class MeleeItemData : ScriptableObject
{
    [Header("Damage Settings")]
    public int damageAmount;
    public float damageDuration;

    [Header("Attack Settings")]
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    public float attackRange = 1f;
    public LayerMask attackLayer;

    [Header("Defend Settings")]
    public bool canBlock;

    [Header("UI Settings")]
    public Sprite icon;

    [Header("Impact Settings")]
    public ImpactEffectData impactEffectData;

    [Header("Animation Settings")]
    public string idleAnimName;
    public string attackAnimName1;
    public string attackAnimName2;
    public string block;
}
