using UnityEngine;

[CreateAssetMenu(fileName = "MeleeItemData", menuName = "ScriptableObjects/MeleeItemData", order = 1)]
public class MeleeItemData : ScriptableObject
{
    [Header("Damage Settings")]
    public int damageAmount;
    public float damageDuration;

    [Header("Attack Settings")]
    public float attackActionInTime = 0.4f;
    public float attackResetInTime = 1f;
    public float attackRange = 1f;
    public LayerMask attackLayer;

    [Header("Defend Settings")]
    public bool canBlock;

    [Header("Physics Settings")]
    public float knockbackForce;

    [Header("UI Settings")]
    public Sprite icon;

    // FIXME: Improve the particle system to support multiple effects
    [Header("Impact Settings")]
    public ImpactEffectData impactEffectData;

    [Header("Animation Settings")]
    public string idleAnimName;
    public string attackAnimName1;
    public string attackAnimName2;
    public string block;
}
