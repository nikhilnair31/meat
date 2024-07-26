using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour 
{
    private ParticleSystem.CollisionModule collisionModule;

    internal ParticleSystem bloodParticleSystem;
    internal List<ParticleCollisionEvent> collisionEvents = new();

    [Header("Decal Properties")]
    [SerializeField] private GameObject damageDecalObject;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask decalLayerMask;
    [SerializeField] private Vector2 randomScaleRange = new (0.5f, 1.5f);
    [SerializeField] private float rotationOffsetRange = 360f;
    [SerializeField] private float decalPivotOffset = 0.05f;
    [SerializeField] private float raycastLength = 5f;
    [SerializeField] private int numberOfRays = 20;
    [SerializeField] private int maxDecals = 20;
    private int currDecals = 0;

    void Start() {
        bloodParticleSystem = GetComponent<ParticleSystem>();
        collisionModule = bloodParticleSystem.collision;
    }  
    
    protected virtual void OnParticleCollision(GameObject other) {
        int numCollisionEvents = bloodParticleSystem.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++) {
            if (other.CompareTag("Ground")) {
                collisionModule.collidesWith &= ~playerLayer;

                if (currDecals < maxDecals) {
                    SpawnBloodDecalsWithSphereRaycasts();
                }
            }
        }
    }

    // TODO: Check if this raycast approach can be improved
    private void SpawnBloodDecalsWithSphereRaycasts() {
        for (int i = 0; i < numberOfRays; i++) {
            Vector3 randomDirection = Random.onUnitSphere;
            Ray ray = new (transform.position, randomDirection);
            
            Debug.DrawRay(transform.position, randomDirection * raycastLength, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastLength, decalLayerMask)) {
                if (currDecals < maxDecals) {
                    Vector3 decalPosition = hit.point + hit.normal * decalPivotOffset;

                    // FIXME: Randomize rotation of decal about the hit normal
                    Quaternion rotation = Quaternion.LookRotation(hit.normal);
                    // Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), hit.normal);
                    // Quaternion finalRotation = randomRotation * rotation;

                    GameObject decalInstance = Instantiate(
                        damageDecalObject, 
                        decalPosition, 
                        rotation
                    );

                    // Randomize the scale of the decal, keeping it square
                    float randomScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
                    decalInstance.transform.localScale = new Vector3(randomScale, randomScale, decalInstance.transform.localScale.z);

                    currDecals++;
                }
            }
        }
    } 
}