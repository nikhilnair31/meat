using UnityEngine;

[System.Serializable]
public class TransformData
{
    private bool wasDestroyed; 

    [Header("Main Settings")]
    public string transformName;
    public Collider collider;
    public Rigidbody rigidbody;
    
    [Header("Effects")]
    [SerializeField] private GameObject spawnedLimbObject;
    [SerializeField] private GameObject damageImpactObject;

    [Header("Data")]
    public float transformDamageMultiplier = 0.8f;
    [SerializeField] private float transformMaxHealth = 100;
    public float transformCurrentHealth;

    [Header("Flags")]
    public bool destroyable;
    [SerializeField] private bool destroyed;

    public void Init(GameObject defaultSpawnedLimbObject, GameObject defaultDamageImpactObject) 
    {
        if(spawnedLimbObject == null) {
            spawnedLimbObject = defaultSpawnedLimbObject;
        }
        if(damageImpactObject == null) {
            damageImpactObject = defaultDamageImpactObject;
        }

        transformCurrentHealth = transformMaxHealth;
    }
    public void UpdateScale()
    {
        if (destroyable)
        {
            if ((destroyed || transformCurrentHealth <= 0) && !wasDestroyed)
            {
                wasDestroyed = true;

                // Spawn spawned limb
                GameObject newLimb = Object.Instantiate(spawnedLimbObject, collider.transform.position, collider.transform.rotation);
                newLimb.name = spawnedLimbObject.name;
                
                // Spawn damage impact
                GameObject impact = Object.Instantiate(damageImpactObject, collider.transform.position, collider.transform.rotation);
                Object.Destroy(impact, 5f);

                // Make enemy ragdoll when any limb is completely destroyed
                var enemyHealth = Helper.GetComponentInParentByTag<EnemyHealth>(collider.transform, "Enemy");
                if(enemyHealth != null) enemyHealth.EnemyDead();

                collider.transform.localScale = Vector3.zero;
                collider.gameObject.SetActive(false);
            }
        }
    }
}
