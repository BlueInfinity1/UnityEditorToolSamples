// A correction tool for undoing the rotation of a SpriteShape object while keeping all the SpriteShape points in their original global positions. 

using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
using System.Collections.Generic;

public class ResetSpriteShapeRotation
{
    [MenuItem("Tools/SpriteShape Rotation Correction Tools/Undo SpriteShape Parent Rotation")]
    private static void UndoSpriteShapeRotation()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Transform transform = obj.transform;

            // Register Undo for the parent transform
            Undo.RecordObject(transform, "Undo Parent Rotation");

            // Dictionary to store global points for each child
            Dictionary<Transform, List<Vector3>> globalPointsDict = new Dictionary<Transform, List<Vector3>>();

            // Step 1: Store global spline points
            foreach (Transform child in transform)
            {
                SpriteShapeController spriteShape = child.GetComponent<SpriteShapeController>();
                if (spriteShape != null)
                {
                    List<Vector3> globalPoints = new List<Vector3>();
                    Spline spline = spriteShape.spline;

                    // Register Undo for the SpriteShapeController
                    Undo.RegisterCompleteObjectUndo(spriteShape, "Undo SpriteShape Modification");

                    for (int i = 0; i < spline.GetPointCount(); i++)
                    {
                        Vector3 globalPoint = child.TransformPoint(spline.GetPosition(i));
                        globalPoints.Add(globalPoint);
                    }
                    globalPointsDict.Add(child, globalPoints);
                }
            }

            // Step 2: Reset rotations if deviation is less than 0.5 degrees
            transform.rotation = Quaternion.Euler(0, 0, 0);
            foreach (Transform child in transform)
            {
                if (Mathf.Abs(child.localEulerAngles.x) < 0.5f && Mathf.Abs(child.localEulerAngles.y) < 0.5f && Mathf.Abs(child.localEulerAngles.z) < 0.5f)
                {
                    // Register Undo for the child transform
                    Undo.RecordObject(child, "Undo Child Rotation");
                    child.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }

            // Step 3: Apply stored spline points in new local space
            foreach (Transform child in transform)
            {
                if (globalPointsDict.TryGetValue(child, out List<Vector3> globalPoints))
                {
                    SpriteShapeController spriteShape = child.GetComponent<SpriteShapeController>();
                    if (spriteShape != null)
                    {
                        Spline spline = spriteShape.spline;
                        for (int i = 0; i < spline.GetPointCount(); i++)
                        {
                            Vector3 localPoint = child.InverseTransformPoint(globalPoints[i]);
                            spline.SetPosition(i, localPoint);
                        }
                        spriteShape.RefreshSpriteShape();
                    }
                }
            }
        }
    }
}
