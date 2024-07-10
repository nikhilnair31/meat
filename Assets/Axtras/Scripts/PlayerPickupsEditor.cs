using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerPickups))]
public class PlayerPickupsEditor : Editor 
{
    private string prefabPath = "Assets/Axtras/Prefabs/";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerPickups playerPickups = (PlayerPickups)target;

        if (GUILayout.Button("Fill Pickup Items List"))
        {
            FillPickupItemsList(playerPickups);
        }
        if (GUILayout.Button("Clear Pickup Items List"))
        {
            ClearPickupItemsList(playerPickups);
        }
    }

    private void FillPickupItemsList(PlayerPickups playerPickups)
    {
        if (playerPickups.playerPickupItemsList.Count == 0)
        {
            foreach (Transform child in playerPickups.pickupHolder)
            {
                // FIXME: Doesn't automatically find the prefab
                // Find the prefab with the same name
                string finalPrefabPath = prefabPath + child.gameObject.name;
                GameObject prefab = (GameObject)Resources.Load(finalPrefabPath, typeof(GameObject));
                Debug.Log($"finalPrefabPath: {finalPrefabPath} | prefab: {prefab}");

                PlayerPickupItems item = new()
                {
                    name = child.gameObject.name,
                    
                    prefab = prefab ? prefab : null,
                    obj = child.gameObject,
                };

                playerPickups.playerPickupItemsList.Add(item);
            }

            Debug.Log($"Added {playerPickups.playerPickupItemsList.Count} items to the pickup items list.");
        }
        else
        {
            Debug.Log("Pickup Items List already full");
        }
    }
    private void ClearPickupItemsList(PlayerPickups playerPickups)
    {
        if (playerPickups.playerPickupItemsList.Count > 0)
        {
            playerPickups.playerPickupItemsList.Clear();
            Debug.Log($"Cleared {playerPickups.playerPickupItemsList.Count} items from the pickup items list.");
        }
        else
        {
            Debug.Log("Pickup Items List already empty");
        }
    }
}