using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int currentHealth;
    
    [Header("Enemy Mesh")]
    public SkinnedMeshRenderer enemySkinnedMeshRenderer;
    public List<LimbHealth> limbHealthList;

    [Header("Enemy Health")]
    [SerializeField] private int maxHealth = 100;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Die()
    {
        // Add death animation or effects here
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }
}
