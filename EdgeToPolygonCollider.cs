using UnityEditor;
using UnityEngine;

public class EdgeToPolygonCollider : Editor
{
    [MenuItem("Tools/Correction Tools/Convert Edge Collider to Polygon Collider")]
    public static void ConvertEdgeToPolygon()
    {
        // Ensure exactly one GameObject is selected
        if (Selection.gameObjects.Length != 1)
        {
            Debug.LogWarning("Please select exactly one GameObject with an EdgeCollider2D.");
            return;
        }

        GameObject selectedObject = Selection.activeGameObject;
        
        EdgeCollider2D edgeCollider = selectedObject.GetComponent<EdgeCollider2D>();
        if (edgeCollider == null)
        {
            Debug.LogWarning("Selected object does not have an EdgeCollider2D: " + selectedObject.name);
            return;
        }

        // Get or add the PolygonCollider2D component
        PolygonCollider2D polyCollider = selectedObject.GetComponent<PolygonCollider2D>();
        if (polyCollider == null)        
            polyCollider = selectedObject.AddComponent<PolygonCollider2D>();
        
        // Convert EdgeCollider2D points to PolygonCollider2D path
        Vector2[] points = edgeCollider.points;
        polyCollider.pathCount = 1;
        polyCollider.SetPath(0, points);

        // Remove the EdgeCollider2D if no longer needed
        DestroyImmediate(edgeCollider);

        Debug.Log("Converted EdgeCollider2D to PolygonCollider2D on object: " + selectedObject.name);
    }
}
