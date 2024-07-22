using UnityEngine;

[CreateAssetMenu(fileName = "ThrowableWeaponData", menuName = "ScriptableObjects/ThrowableWeaponData", order = 1)]
public class ThrowableWeaponData : ScriptableObject
{
    [Header("Physics Settings")]
    public float throwForce;

    [Header("Damage Settings")]
    public int damageAmount;
    public float damageDuration;

    [Header("UI Settings")]
    public Sprite weaponIcon;
}