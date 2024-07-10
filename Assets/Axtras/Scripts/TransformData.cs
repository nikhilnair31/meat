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
    [SerializeField] private int transformMaxHealth = 100;
    public int transformCurrentHealth;

    [Header("Flags")]
    [SerializeField] private bool destroyable;
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
                collider.transform.localScale = Vector3.zero;
                collider.gameObject.SetActive(false);

                // Spawn spawned limb
                GameObject newLimb = Object.Instantiate(spawnedLimbObject, collider.transform.position, collider.transform.rotation);
                newLimb.name = spawnedLimbObject.name;
                
                // Spawn damage impact
                GameObject impact = Object.Instantiate(damageImpactObject, collider.transform.position, collider.transform.rotation);
                Object.Destroy(impact, 5f);

                wasDestroyed = true;
            }
            // else if (!destroyed && wasDestroyed)
            // {
            //     collider.transform.localScale = Vector3.one;
            //     collider.gameObject.SetActive(true);
            //     wasDestroyed = false;
            // }
        }
    }
}
