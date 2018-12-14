using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera eventCamera;
    public Transform target;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(target.position, eventCamera.transform.position);
    }

    private void OnValidate()
    {
        if (!target)
        {
            target = transform;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (!eventCamera)
        {
            Debug.LogWarning("Event Camera has not been assigned!");
            
            return;
        }

        target.LookAt(eventCamera.transform);
    }
}
