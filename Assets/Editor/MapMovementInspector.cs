using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(MapMovement))]
[CanEditMultipleObjects]
public class MapMovementInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();
        MapMovement targetScript = target as MapMovement;

        List<string> excludedProperties = new List<string>();

        if (!targetScript.ignoreManager)
        {
            excludedProperties.Add("segmentEnabled");
        }

        DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());

        serializedObject.ApplyModifiedProperties();
    }
}
