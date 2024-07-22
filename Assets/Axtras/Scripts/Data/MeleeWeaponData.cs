using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponData", menuName = "ScriptableObjects/MeleeWeaponData", order = 1)]
public class MeleeWeaponData : ScriptableObject
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
    public Sprite weaponIcon;

    [Header("Impact Settings")]
    public ImpactEffectData impactEffectData;
}
