using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveParentPivotToPosition : ScriptableWizard
{
    public Vector3 targetPosition = Vector3.zero;

    [MenuItem("Tools/Move Pivot to Position...")]
    static void MoveSelectPivotToPosition()
    {
        DisplayWizard<MoveParentPivotToPosition>("Move Pivot to Position...", "Move Pivot");
    }

    private void OnWizardCreate()
    {
        Transform selectedTransform = Selection.gameObjects[0].transform;
        List<Transform> allChildrenTransforms = new List<Transform>();

        // Collect all transforms and move all of them out of parent
        for (int v = 0; v < selectedTransform.childCount; v++)
        {
            allChildrenTransforms.Add(selectedTransform.GetChild(v));
            selectedTransform.GetChild(v).parent = null;
        }

        // Reposition selected transform to target
        selectedTransform.position = targetPosition;

        // Put all children back to parent;
        foreach (Transform t in allChildrenTransforms)
        {
            t.parent = selectedTransform;
        }
    }

    [MenuItem("Tools/Move Pivot to Position...", true)]
    static bool MovePivotToPositionValidation()
    {
        // Only be vailid if one item is selected
        if (Selection.gameObjects.Length != 1) return false;

        // Makes sure there are more than 1 child in selected transform
        if (Selection.transforms[0].childCount <= 1) return false;

        // Valid :)
        return true;
    }
}
