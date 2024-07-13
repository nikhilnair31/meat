using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour 
{
    private MenuManager menuManager;

    [Header("Health Properties")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currHealth;

    [Header("Effects")]
    public ParticleSystem fireEffect;

    private void Start() {
        menuManager = FindObjectOfType<MenuManager>();

        currHealth = maxHealth;

        UpdateHealthUI();
    }

    public void AddHealth(int amount, float duration) {
        StartCoroutine(AddHealthOverTime(amount, duration));
    }
    private IEnumerator AddHealthOverTime(int amount, float duration) {
        float elapsedTime = 0f;
        int initialHealth = currHealth;
        int targetHealth = Mathf.Clamp(currHealth + amount, 0, maxHealth);

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            currHealth = Mathf.RoundToInt(Mathf.Lerp(initialHealth, targetHealth, elapsedTime / duration));

            UpdateHealthUI();

            if (currHealth > maxHealth) {
                currHealth = maxHealth;
                break;
            }
            
            yield return null;
        }

        currHealth = Mathf.Min(currHealth, maxHealth);
        UpdateHealthUI();
    }
    
    public void DiffHealth(int amount, float duration) {
        StartCoroutine(DiffHealthOverTime(amount, duration));
    }
    private IEnumerator DiffHealthOverTime(int amount, float duration) {
        float elapsedTime = 0f;
        int initialHealth = currHealth;
        int targetHealth = Mathf.Clamp(currHealth - amount, 0, maxHealth);

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            currHealth = Mathf.RoundToInt(Mathf.Lerp(initialHealth, targetHealth, elapsedTime / duration));

            UpdateHealthUI();

            if (currHealth <= 0) {
                currHealth = 0;
                Die();
                break;
            }

            yield return null;
        }

        currHealth = Mathf.Min(currHealth, maxHealth);
        UpdateHealthUI();
    }
    
    private void Die() {
        // Add death animation or effects here
        menuManager.ShowDeathMenu();
    }
    
    private void UpdateHealthUI() {
        healthText.text = $"{currHealth}/{maxHealth}";
    }
}