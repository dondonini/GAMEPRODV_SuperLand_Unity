using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SegmentBottom))]
[CanEditMultipleObjects]
public class SegmentBottomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SegmentBottom targetScript = target as SegmentBottom;

        List<string> excludedProperties = new List<string>();

        if (targetScript.setBottomEndToDefault)
        {
            excludedProperties.Add("bottomEndPosition");
        }

        DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());

        serializedObject.ApplyModifiedProperties();
    }
}
