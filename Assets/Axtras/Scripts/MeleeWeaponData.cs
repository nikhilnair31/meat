using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponData", menuName = "ScriptableObjects/MeleeWeaponData", order = 1)]
public class MeleeWeaponData : ScriptableObject
{
    [Header("Damage Settings")]
    public int damageAmount;
    public float damageDuration;

    [Header("Camera Shake Settings")]
    public CameraShakeData cameraShakeData;

    [Header("Audio Settings")]
    public AudioClip attackSound;
}
