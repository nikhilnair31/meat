using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour 
{
    private MenuManager menuManager;

    [Header("Health Properties")]
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currHealth;

    [Header("Health Properties")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private RectTransform healthBarRect;
    private float healthBarWidth;

    [Header("Effects")]
    public ParticleSystem fireEffect;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip healClip;
    [SerializeField] private float healClipVoume =2f;

    private void Start() {
        menuManager = FindObjectOfType<MenuManager>();

        currHealth = maxHealth;
        healthBarWidth = healthBarRect.sizeDelta.x;

        UpdateHealthEffects(null);
    }

    public void AddHealth(float amount, float duration) {
        StartCoroutine(AddHealthOverTime(amount, duration));
    }
    private IEnumerator AddHealthOverTime(float amount, float duration) {
        float elapsedTime = 0f;
        float initialHealth = currHealth;
        float targetHealth = Mathf.Clamp(currHealth + amount, 0, maxHealth);

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            currHealth = Mathf.Lerp(initialHealth, targetHealth, elapsedTime / duration);

            UpdateHealthEffects(healClip);

            if (currHealth > maxHealth) {
                currHealth = maxHealth;
                break;
            }
            
            yield return null;
        }

        currHealth = Mathf.Min(currHealth, maxHealth);
        UpdateHealthEffects(healClip);
    }
    
    public void DiffHealth(float amount, float duration) {
        StartCoroutine(DiffHealthOverTime(amount, duration));
    }
    private IEnumerator DiffHealthOverTime(float amount, float duration) {
        float elapsedTime = 0f;
        float initialHealth = currHealth;
        float targetHealth = Mathf.Clamp(currHealth - amount, 0, maxHealth);

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            currHealth = Mathf.Lerp(initialHealth, targetHealth, elapsedTime / duration);

            UpdateHealthEffects(hurtClip);

            if (currHealth <= 0) {
                currHealth = 0;
                Die();
                break;
            }

            yield return null;
        }

        currHealth = Mathf.Min(currHealth, maxHealth);
        UpdateHealthEffects(hurtClip);
    }
    
    private void Die() {
        // Add death animation or effects here
        menuManager.ShowDeathMenu();
    }
    
    private void UpdateHealthEffects(AudioClip clip) {
        if (clip != null) {
            GetComponent<AudioSource>().PlayOneShot(clip, healClipVoume);
        }

        healthText.text = $"{(int)currHealth}/{maxHealth}";
        healthBarRect.sizeDelta = new Vector2(
            healthBarWidth * currHealth / maxHealth, 
            healthBarRect.sizeDelta.y
        );
    }
}