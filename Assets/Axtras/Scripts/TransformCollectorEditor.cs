using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TransformCollector))]
public class TransformCollectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TransformCollector transformCollector = (TransformCollector)target;

        if (GUILayout.Button("Fill Child Transforms"))
        {
            CollectChildTransforms(transformCollector);
        }
    }

    private void CollectChildTransforms(TransformCollector transformCollector)
    {
        if(transformCollector.transformDataList.Count == 0)
        {
            transformCollector.transformDataList.Clear();

            Collider[] colliders = transformCollector.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent<Rigidbody>(out var rigidbody))
                {
                    TransformData data = new()
                    {
                        transformName = collider.transform.name,
                        collider = collider,
                        rigidbody = rigidbody
                    };
                    transformCollector.transformDataList.Add(data);
                }
            }

            EditorUtility.SetDirty(transformCollector);
        }
        else {
            Debug.Log("TransformCollector already has data");
        }
    }
}
