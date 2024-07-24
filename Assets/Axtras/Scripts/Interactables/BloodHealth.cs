using System.Collections.Generic;
using UnityEngine;

public class BloodHealth : MonoBehaviour 
{
    private ParticleSystem bloodParticleSystem;
    private ParticleSystem.CollisionModule collisionModule;
    private List<ParticleCollisionEvent> collisionEvents = new();

    [Header("Heal Properties")]
    [SerializeField] private float healAmount = 20;
    [SerializeField] private float healTime = 0f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Decal Properties")]
    [SerializeField] private GameObject damageDecalObject;
    [SerializeField] private LayerMask decalLayerMask;
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
    
    private void OnParticleCollision(GameObject other) {
        int numCollisionEvents = bloodParticleSystem.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++) {
            ParticleCollisionEvent collisionEvent = collisionEvents[i];

            if (other.CompareTag("Player")) {
                if (other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth)) {
                    Debug.Log("Player healing with blood");
                    playerHealth.AddHealth(healAmount, healTime);
                }
            }

            if (other.CompareTag("Ground")) {
                collisionModule.collidesWith &= ~playerLayer;

                if (currDecals < maxDecals) {
                    SpawnBloodDecalsWithSphereRaycasts();
                }
            }
        }
    }

    private void SpawnBloodDecalsWithSphereRaycasts() {
        for (int i = 0; i < numberOfRays; i++) {
            Vector3 randomDirection = Random.onUnitSphere;
            Ray ray = new (transform.position, randomDirection);
            
            Debug.DrawRay(transform.position, randomDirection * raycastLength, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastLength, decalLayerMask)) {
                if (currDecals < maxDecals) {
                    Vector3 decalPosition = hit.point + hit.normal * decalPivotOffset;

                    // FIXME: Randomize rotation of decal about the hit normal
                    Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), hit.normal);
                    Quaternion finalRotation = Quaternion.LookRotation(hit.normal) * randomRotation;

                    Instantiate(
                        damageDecalObject, 
                        decalPosition, 
                        Quaternion.LookRotation(hit.normal)
                    );

                    currDecals++;
                }
            }
        }
    }
}