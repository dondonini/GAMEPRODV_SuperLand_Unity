using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UI3DAttachment))]
public class UI3DAttachmentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UI3DAttachment myScript = (UI3DAttachment)target;
        if (GUILayout.Button("Force Attach"))
        {
            myScript.Attach3DUIObject();

            myScript.Set3DUIPosition(myScript.rectTransform.position);

            float xScale = myScript.rectTransform.rect.width / 100.0f;
            float yScale = myScript.rectTransform.rect.height / 100.0f;
            float scaleAmount = Mathf.Min(xScale, yScale);

            myScript.Set3DUIScale(scaleAmount);
        }
    }
}
