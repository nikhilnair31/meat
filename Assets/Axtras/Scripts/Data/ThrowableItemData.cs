using UnityEngine;

[CreateAssetMenu(fileName = "ThrowableItemData", menuName = "ScriptableObjects/ThrowableItemData", order = 1)]
public class ThrowableItemData : ScriptableObject
{
    [Header("Physics Settings")]
    public float throwForce;

    [Header("Damage Settings")]
    public int damageAmount;
    public float damageDuration;

    [Header("UI Settings")]
    public Sprite icon;

    [Header("Animation Settings")]
    public string holdingAnimnName;
    public string throwingAnimName;
    public float throwSpeed;
    public float throwDelay;
}