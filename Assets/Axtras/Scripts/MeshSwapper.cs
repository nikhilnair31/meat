using UnityEngine;

public class MeshSwapper : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer; // The SkinnedMeshRenderer component on your GameObject
    public Mesh[] meshes; // An array of meshes you want to swap between

    private int currentMeshIndex = 0;

    void Start()
    {
        // Ensure the SkinnedMeshRenderer is assigned
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        }
    }

    void Update()
    {
        // For demonstration purposes, swap mesh on key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwapMesh();
        }
    }

    void SwapMesh()
    {
        // Increment the current mesh index and wrap around if necessary
        currentMeshIndex = (currentMeshIndex + 1) % meshes.Length;

        // Assign the new mesh
        skinnedMeshRenderer.sharedMesh = meshes[currentMeshIndex];
    }
}
