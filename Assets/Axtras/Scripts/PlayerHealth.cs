using UnityEngine;
using TMPro;

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

    public void AddHealth(int amount) {
        currHealth += amount;
        if (currHealth > maxHealth) {
            currHealth = maxHealth;
        }

        UpdateHealthUI();
    }
    public void DiffHealth(int amount) {
        currHealth -= amount;
        if (currHealth <= 0) {
            currHealth = 0;
            Die();
        }

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