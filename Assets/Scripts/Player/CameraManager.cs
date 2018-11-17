using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public float cameraDistance = 100.0f;
    [Range(0, 180)]
    public float cameraAngleHeight = 30.0f;
    public float cameraAngleRotation = 45.0f;
    public float speedDamp = 0.001f;
    public Vector3 targetOffset = Vector3.zero;
    public float zoomPadding = 20;
    public Transform mainCamera;
    public Transform cameraAim;
    [SerializeField]
    private bool showDebug = false;

    private GameManager gm;

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/

    private Vector3 rigV = Vector3.zero;
    private Vector3 mainCameraV = Vector3.zero;
    private Vector3 cameraAimV = Vector3.zero;
    private float rigRotationV = 0.0f;
    private Bounds tempBounds;

    // This only enables if there are more than one subject in the shot
    private bool autoZoom = false;

    private void OnValidate()
    {
        if (cameraAngleRotation > 360.0f)
        {
            cameraAngleRotation = 0.0f;
        }
        else if (cameraAngleRotation < 0.0f)
        {
            cameraAngleRotation = 360.0f;
        }
    }

    // Use this for initialization
    void Start () {
        // Caching
		gm = GameManager.GetInstance();
	}
	
	// Update is called once per frame
	void LateUpdate () {

        Vector3 targetPosition = SolveTargetPosition();

        // SmoothDamp rig position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref mainCameraV, speedDamp);

        // SmoothDamp camera position
        mainCamera.transform.localPosition = Vector3.SmoothDamp(mainCamera.transform.localPosition, CalculateCameraPosition(Vector3.zero), ref rigV, speedDamp);

        // SmoothDamp target position
        cameraAim.position = Vector3.SmoothDamp(cameraAim.position, targetPosition, ref cameraAimV, speedDamp);

        // SmoothDamp rig rotation
        Vector3 rigEulerAngle = transform.eulerAngles;
        rigEulerAngle.y = Mathf.SmoothDampAngle(rigEulerAngle.y, cameraAngleRotation, ref rigRotationV, speedDamp);
        transform.eulerAngles = rigEulerAngle;

        mainCamera.LookAt(cameraAim);
	}

    private void OnDrawGizmosSelected()
    {
        if (!showDebug) return;
        if (!Application.isPlaying) return;

        // Camera to target smooth
        Gizmos.color = Color.red;
        Gizmos.DrawLine(mainCamera.position, cameraAim.position);
        Gizmos.DrawCube(cameraAim.position,new Vector3(0.1f, 0.1f, 0.1f));

        // Camera to target target positions
        Gizmos.color = Color.green;
        Gizmos.DrawLine(CalculateCameraPosition(SolveTargetPosition()), SolveTargetPosition());
        Gizmos.DrawCube(SolveTargetPosition(), new Vector3(0.1f, 0.1f, 0.1f));
        Gizmos.DrawCube(CalculateCameraPosition(SolveTargetPosition()), new Vector3(0.1f, 0.1f, 0.1f));

        // Target visuals
        Gizmos.color = Color.blue;
        if (gm.subjects.Count == 1)
        {
            Gizmos.DrawCube(gm.subjects[0].position,new Vector3(0.1f, 0.1f, 0.1f));
        }
        else if (gm.subjects.Count == 2)
        {
            Transform sub1 = gm.subjects[0];
            Transform sub2 = gm.subjects[1];

            Gizmos.DrawLine(sub1.position, sub2.position);
            Gizmos.DrawCube(sub1.position,new Vector3(0.1f, 0.1f, 0.1f));
            Gizmos.DrawCube(sub2.position,new Vector3(0.1f, 0.1f, 0.1f));
        }
        else if (gm.subjects.Count >= 3)
        {
            Gizmos.DrawWireCube(tempBounds.center, tempBounds.size);
            for (int i = 0; i < gm.subjects.Count; i++)
            {
                Gizmos.DrawCube(gm.subjects[i].position,new Vector3(0.1f, 0.1f, 0.1f));
            }
        }
        
        
    }

    Vector3 SolveTargetPosition()
    {

        if (tempBounds != null)
        {
            tempBounds.center = Vector3.zero;
            tempBounds.size = Vector3.zero;
        }

        // Solving for singular target
        if (gm.subjects.Count == 1)
        {
            // Disable auto zoom
            autoZoom = false;

            return gm.subjects[0].position + targetOffset;
        }

        // Solving for dual targets
        else if (gm.subjects.Count == 2)
        {
            // Enable auto zoom
            autoZoom = true;

            // Calculate middle position
            Vector3 midPoint = Vector3.Lerp(gm.subjects[0].position, gm.subjects[1].position, 0.5f);

            return midPoint + targetOffset;
        }

        // Solving for multiple targets
        else if (gm.subjects.Count >= 3)
        {
            // Enable auto zoom
            autoZoom = true;

            tempBounds = new Bounds(gm.subjects[0].position, Vector3.zero);

            for (int i = 1; i < gm.subjects.Count; i++)
            {
                tempBounds.Encapsulate(gm.subjects[i].position);
            }
            
            return tempBounds.center + targetOffset;
        }

        return Vector3.zero;
    }

    Vector3 CalculateCameraPosition(Vector3 target)
    {
        // Calculate hypotenuse
        float hyp = cameraDistance;

        // Only for multi subject situations
        if (autoZoom)
        {
            // Calculate zoom to accommodate 2 subjects
            if (gm.subjects.Count == 2)
            {
                // Distance between both subjects
                float distBetween = Vector3.Distance(gm.subjects[0].position, gm.subjects[1].position) + zoomPadding;

                hyp = (distBetween * 0.5f) * Mathf.Tan((CalculateFOVHeightFromFOVWidth(Camera.main.fieldOfView) * 0.5f) * Mathf.Deg2Rad);
            }

            // Calculate zoom to accommodate more than 2 subjects
            else
            {
                if (tempBounds != null)
                {
                    float maxDistance = Vector3.Distance(tempBounds.center, tempBounds.max) * 2 + zoomPadding;

                    hyp = (maxDistance * 0.5f) * Mathf.Tan((CalculateFOVHeightFromFOVWidth(Camera.main.fieldOfView) * 0.5f) * Mathf.Deg2Rad);
                }
            }
        }

        // Calculate adjacent
        float adj = hyp * Mathf.Cos(cameraAngleHeight * Mathf.Deg2Rad);

        // Calculate opposite
        float opp = Mathf.Sqrt(Mathf.Pow(hyp, 2) - Mathf.Pow(adj, 2));

        // Solving camera position
        Vector3 newPosition = target;

        newPosition.y += opp;
        newPosition.z -= adj;

        return newPosition;
    }

    float CalculateFOVHeightFromFOVWidth(float fovWidth)
    {
        Vector2 tempResolution = new Vector2(Screen.width, Screen.height);
        
        // Calculate adjacent
        float adj = (tempResolution.x * 0.5f) * Mathf.Tan((fovWidth * 0.5f) * Mathf.Deg2Rad);

        float fovHeight = (((tempResolution.y * 0.5f) / adj) * Mathf.Rad2Deg) * 2.0f;

        return fovHeight;
    }
}
