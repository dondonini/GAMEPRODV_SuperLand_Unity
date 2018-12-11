using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SegmentBottom : MonoBehaviour
{
    public Transform segmentMesh;

    public float topOffset = 0.0f;
    public bool setBottomEndToDefault = true;
    public float bottomEndPosition = -50.0f;

    /************************************************************************/
    /* References                                                           */
    /************************************************************************/

    const float DEFAULT_BOTTOM_END = -50.0f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(CalculateTopPosition(), new Vector3(0.1f, 0.1f, 0.1f));
        Gizmos.DrawCube(CalculateBottomPosition(), new Vector3(0.1f, 0.1f, 0.1f));

        Gizmos.DrawLine(CalculateBottomPosition(), CalculateTopPosition());
    }

    private void OnValidate()
    {
        if (bottomEndPosition > CalculateTopPosition().y - 0.1f)
        {
            bottomEndPosition = CalculateTopPosition().y - 0.1f;
        }
    }

    private void Update()
    {

        Vector3 topPosition = CalculateTopPosition();
        Vector3 bottomPosition = CalculateBottomPosition();


        // Set mesh position
        segmentMesh.position = Vector3.Lerp(topPosition, bottomPosition, 0.5f);

        // Set mesh scale
        Vector3 newScale = segmentMesh.localScale;
        newScale.z = Vector3.Distance(bottomPosition, topPosition) * 2.0f;
        segmentMesh.localScale = newScale;
    }

    Vector3 CalculateTopPosition()
    {
        Vector3 topPosition = transform.position;
        topPosition.y += topOffset;

        return topPosition;
    }

    Vector3 CalculateBottomPosition()
    {
        Vector3 bottomPosition = CalculateTopPosition();

        // Default to -50.0f position for the bottom face or use custom
        if (setBottomEndToDefault)
        {
            bottomPosition.y = DEFAULT_BOTTOM_END;
        }
        else
        {
            bottomPosition.y = bottomEndPosition;
        }

        // Prevent bottom face from going over the top
        if (bottomPosition.y > CalculateTopPosition().y)
        {
            bottomPosition.y = CalculateTopPosition().y - 0.1f;
        }
        
        return bottomPosition;
    }
}
