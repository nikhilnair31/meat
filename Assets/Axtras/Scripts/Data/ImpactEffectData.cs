using UnityEngine;

[CreateAssetMenu(fileName = "ImpactEffectData", menuName = "ScriptableObjects/ImpactEffectData", order = 1)]
public class ImpactEffectData : ScriptableObject
{
    [Header("Shake Effects")]
    public float hurtShakeMagnitude = 4.0f;
    public float hurtShakeDuration = 0.4f;
    public float hurtShakeMultiplier = 1.2f;

    [Header("Sound Effects")]
    public AudioClip impactClip;
    public float impactVolume = 1.0f;
    public bool randPitch = false;

    [Header("Visual Effects")]
    public GameObject impactParticlePrefab;
    public bool destroyAfterInstantiation = false;
    public float destroyDelay = 1.0f;
}