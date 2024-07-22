using UnityEngine;

[CreateAssetMenu(fileName = "SwingableWeaponData", menuName = "ScriptableObjects/SwingableWeaponData", order = 1)]
public class SwingableWeaponData : ScriptableObject
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
    public Sprite weaponIcon;
}