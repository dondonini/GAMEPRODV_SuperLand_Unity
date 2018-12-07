using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuItems 
{
    [MenuItem("Tools/Center Parent to Children")]
    static void CenterParentToChildren()
    {
        Transform selectedTransform = Selection.gameObjects[0].transform;
        List<Transform> allChildrenTransforms = new List<Transform>();
        Vector3 totalVectors = Vector3.zero;

        // Add all children positions together and collect all transforms
        for (int v = 0; v < selectedTransform.childCount; v++)
        {
            totalVectors += selectedTransform.GetChild(v).position;
            allChildrenTransforms.Add(selectedTransform.GetChild(v));
        }

        // Calculate transform average
        Vector3 positionAverage = totalVectors / selectedTransform.childCount;

        // Save undo
        Undo.SetCurrentGroupName("Centered Parent Pivot to Children");

        // Move all transforms out of parent
        foreach (Transform t in allChildrenTransforms)
        {
            Undo.SetTransformParent(t.parent, null, "Centered Parent Pivot to Children");
            Undo.IncrementCurrentGroup();
        }

        // Reposition selected transform to center
        Undo.RecordObject(selectedTransform, "Centered Parent Pivot to Children");
        selectedTransform.position = positionAverage;
        Undo.IncrementCurrentGroup();

        // Put all children back to parent;
        foreach (Transform t in allChildrenTransforms)
        {
            Undo.SetTransformParent(t.parent, selectedTransform, "Centered Parent Pivot to Children");
            Undo.IncrementCurrentGroup();
        }
    }

    [MenuItem("Tools/Center Parent to Children", true)]
    static bool CenterParentToChildrenValidation()
    {
        // Only be vailid if one item is selected
        if (Selection.gameObjects.Length != 1) return false;

        // Makes sure there are more than 1 child in selected transform
        if (Selection.transforms[0].childCount <= 1) return false;

        // Valid :)
        return true;
    }
}
