using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SegmentBottom : MonoBehaviour
{
    public Transform segmentMesh;

    public float topOffset = 0.0f;

    /************************************************************************/
    /* References                                                           */
    /************************************************************************/

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(CalculateTopPosition(), new Vector3(0.1f, 0.1f, 0.1f));
        Gizmos.DrawCube(CalculateBottomPosition(), new Vector3(0.1f, 0.1f, 0.1f));

        Gizmos.DrawLine(CalculateBottomPosition(), CalculateTopPosition());
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
        bottomPosition.y = -50.0f;

        return bottomPosition;
    }
}
