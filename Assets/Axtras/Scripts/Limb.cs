using UnityEngine;

public class Limb : MonoBehaviour
{
    private Enemy enemy;
    private bool limbSpawned = false;

    [Header("Limb Properties")]
    public int limbStrengthDamage = 10;
    [SerializeField] private int limbCurrentHealth = 10;

    [Header("Effects")]
    [SerializeField] private GameObject hitImpactParticlesPrefab;
    [SerializeField] private GameObject finalImpactParticlesPrefab;

    private void Start() {
        enemy = Helper.GetComponentInParentByTag<Enemy>(this.transform, "Enemy");
    }

    public void TakeLimbDamage(int amount) {
        limbCurrentHealth -= amount;
        Debug.Log(transform.name + " took " + amount + " damage. Remaining health: " + limbCurrentHealth);
                
        GameObject hitImpactParticles = Instantiate(hitImpactParticlesPrefab, transform.position, Quaternion.identity);
        Destroy(hitImpactParticles, 1f);

        if (limbCurrentHealth <= 0)
        {
            GameObject finalImpactParticles = Instantiate(finalImpactParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(finalImpactParticles, 15f);

            Debug.Log(transform.name + " is destroyed.");

            // Add limb-specific destruction logic here (e.g., disable limb, play animation)
            if(!limbSpawned) 
            {
                MeshForSwap();
            }
        }
    }

    private void MeshForSwap()
    {
        Mesh limbMesh = null;
        GameObject limbPrefab = null;

        foreach (var limbHealthItem in enemy.limbHealthList)
        {
            Debug.Log($"Comparing {gameObject.name} to {limbHealthItem.limbName}");

            if(gameObject.name.Contains(limbHealthItem.limbName)) 
            {
                limbMesh = limbHealthItem.limbMesh;
                limbPrefab = limbHealthItem.limbPrefab;
            }
        }
        enemy.enemySkinnedMeshRenderer.sharedMesh = limbMesh;
        Instantiate(limbPrefab, transform.position, transform.rotation);
        limbSpawned = true;
    }
}
