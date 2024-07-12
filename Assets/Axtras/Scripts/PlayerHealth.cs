using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Rendering;

public class PlayerHealth : MonoBehaviour 
{
    private MenuManager menuManager;

    [Header("Health Properties")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currHealth;

    private void Start() {
        menuManager = FindObjectOfType<MenuManager>();

        currHealth = maxHealth;

        UpdateHealthUI();
    }

    public void AddHealth(int amount, float duration) {
        StartCoroutine(AddHealthOverTime(amount, duration));
    }
    private IEnumerator AddHealthOverTime(int amount, float duration) {
        float amountPerSecond = amount / duration;
        float totalAdded = 0;

        while (totalAdded < amount) {
            float increment = amountPerSecond * Time.deltaTime;
            currHealth += (int)increment;
            totalAdded += increment;

            if (currHealth > maxHealth) {
                currHealth = maxHealth;
                break;
            }

            UpdateHealthUI();
            yield return null;
        }

        // Ensure health is correctly clamped to maxHealth after loop
        currHealth = Mathf.Min(currHealth, maxHealth);
        UpdateHealthUI();
    }
    
    public void DiffHealth(int amount, float duration) {
        StartCoroutine(DiffHealthOverTime(amount, duration));
    }
    private IEnumerator DiffHealthOverTime(int amount, float duration) {
        float amountPerSecond = amount / duration;
        float totalDiffed = amount;

        while (totalDiffed >= 0) {
            float increment = amountPerSecond * Time.deltaTime;
            currHealth -= (int)increment;
            totalDiffed -= increment;

            if (currHealth <= 0) {
                currHealth = 0;
                Die();
                break;
            }

            UpdateHealthUI();
            yield return null;
        }

        // Ensure health is correctly clamped to maxHealth after loop
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