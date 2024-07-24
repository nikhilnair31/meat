using System.Collections;
using UnityEngine;

public class Fire : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private Fireplace fireplace;
    private bool playerInFire = false;
    private float damageTimer = 0f;

    [Header("Damage Properties")]
    [SerializeField] private int initialDamage = 50;
    [SerializeField] private int continuousDamage = 10;
    [SerializeField] private float damageDuration = 5f;
    [SerializeField] private ParticleSystem fireParticleSystem;

    [Header("Camera Shake Effects")]
    [SerializeField] private ImpactEffectData impactEffectData;

    void Start() {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        fireplace = GetComponent<Fireplace>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && fireplace.isOn) {
            playerInFire = true;
            ApplyDamage(initialDamage);
            StartCoroutine(ContinuousDamage());
        }
    }
    void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") && playerInFire && fireplace.isOn) {
            damageTimer += Time.deltaTime;
            if (damageTimer >= 1f) {
                ApplyDamage(continuousDamage);
                damageTimer = 0f;
            }
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && fireplace.isOn) {
            playerInFire = false;
            StartCoroutine(ApplyDamageOverTime(damageDuration));
        }
    }

    IEnumerator ContinuousDamage() {
        while (playerInFire) {
            ApplyDamage(continuousDamage);
            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator ApplyDamageOverTime(float duration) {
        float timer = 0f;
        while (timer < duration) {
            ApplyDamage(continuousDamage);
            timer += 1f;
            yield return new WaitForSeconds(1f);
        }
        EnableDisableFireEffect(false);
    }

    private void ApplyDamage(int damage) {
        playerHealth.DiffHealth(
            true,
            damage, 
            0.01f
        );
        Helper.CameraShake(
            impactEffectData.hurtShakeMagnitude, 
            impactEffectData.hurtShakeDuration, 
            impactEffectData.hurtShakeMultiplier
        );
        EnableDisableFireEffect(true);
    }

    private void EnableDisableFireEffect(bool enableStatus) {
        if (playerHealth.fireEffect != null) {
            if (enableStatus) {
                playerHealth.fireEffect.Play();
            }
            else {
                playerHealth.fireEffect.Stop();
            }
        }
    }
}
