using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public float cameraDistance = 100.0f;
    [Range(0, 180)]
    public float cameraAngleHeight = 30.0f;
    public float cameraAngleRotation = 45.0f;
    public float cameraMovementDamp = 1.0f;
    public float cameraRotationDamp = 0.2f;
    public Vector3 targetOffset = Vector3.zero;
    public float zoomPadding = 20;
    public float focusThreshold = 50.0f;
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
    private Transform[] subjectsInFocus;

    // This only enables if there are more than one subject in the shot
    private bool autoZoom = false;

    private void OnValidate()
    {
        AdjustCameraRotation(0.0f);
    }

    // Use this for initialization
    void Start () {
        // Caching
		gm = GameManager.GetInstance();
	}

    private void Update()
    {
        UpdateInput();
    }

    // Update is called once per frame
    void LateUpdate () {

        subjectsInFocus = UpdateSubjectsInFocus();

        Vector3 targetPosition = SolveTargetPosition();

        // SmoothDamp rig position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref mainCameraV, cameraMovementDamp);

        // SmoothDamp camera position
        mainCamera.transform.localPosition = Vector3.SmoothDamp(mainCamera.transform.localPosition, CalculateCameraPosition(Vector3.zero), ref rigV, cameraMovementDamp);

        // SmoothDamp target position
        cameraAim.position = Vector3.SmoothDamp(cameraAim.position, targetPosition, ref cameraAimV, cameraMovementDamp);

        // SmoothDamp rig rotation
        Vector3 rigEulerAngle = transform.eulerAngles;
        rigEulerAngle.y = Mathf.SmoothDampAngle(rigEulerAngle.y, cameraAngleRotation, ref rigRotationV, cameraRotationDamp);
        transform.eulerAngles = rigEulerAngle;

        // Aim camera at smooth target
        mainCamera.LookAt(cameraAim);
	}

    /// <summary>
    /// 
    /// </summary>
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
        if (subjectsInFocus.Length == 1)
        {
            Gizmos.DrawCube(subjectsInFocus[0].position,new Vector3(0.1f, 0.1f, 0.1f));
        }
        else if (subjectsInFocus.Length == 2)
        {
            Transform sub1 = subjectsInFocus[0];
            Transform sub2 = subjectsInFocus[1];

            Gizmos.DrawLine(sub1.position, sub2.position);
            Gizmos.DrawCube(sub1.position,new Vector3(0.1f, 0.1f, 0.1f));
            Gizmos.DrawCube(sub2.position,new Vector3(0.1f, 0.1f, 0.1f));
        }
        else if (subjectsInFocus.Length >= 3)
        {
            Gizmos.DrawWireCube(tempBounds.center, tempBounds.size);
            for (int i = 0; i < subjectsInFocus.Length; i++)
            {
                Gizmos.DrawCube(subjectsInFocus[i].position,new Vector3(0.1f, 0.1f, 0.1f));
            }
        }
        
        
    }

    /// <summary>
    /// Update input interactions
    /// </summary>
    void UpdateInput()
    {
        //////////////////////////////////////////////////////////////////////////
        /// Keyboard and Mouse
        /// 

        /************************************************************************/
        /* Camera Rotation                                                      */
        /************************************************************************/

        if (Input.GetButton("Fire2"))
        {
            if (Input.GetAxis("Mouse X") != 0.0f)
            {
                AdjustCameraRotation(Input.GetAxis("Mouse X"));
            }
        }

        //////////////////////////////////////////////////////////////////////////
        /// Controller
        /// 

        /************************************************************************/
        /* Camera Rotation                                                      */
        /************************************************************************/

        if (Input.GetAxis("Horizontal2") != 0.0f)
        {
            AdjustCameraRotation(Input.GetAxis("Horizontal2"));
        }
    }

    void AdjustCameraRotation(float _adjustment)
    {
        cameraAngleRotation += _adjustment;

        if (cameraAngleRotation > 360.0f)
        {
            cameraAngleRotation = 0.0f;
        }
        else if (cameraAngleRotation < 0.0f)
        {
            cameraAngleRotation = 360.0f;
        }
    }

    Transform[] UpdateSubjectsInFocus()
    {
        if (gm.subjects.Count <= 2)
        {
            return gm.subjects.ToArray();
        }

        List<Transform> result = new List<Transform>();

        for (int s = 0; s < gm.subjects.Count; s++)
        {
            float totalDistance = 0;
            Transform currentTrans = gm.subjects[s];

            for (int other = 0; other < gm.subjects.Count; other++)
            {
                if (currentTrans == gm.subjects[other])
                    continue;

                float distanceFrom = Vector3.Distance(currentTrans.position, gm.subjects[other].position);
                
                totalDistance += distanceFrom;
            }

            float distanceAvg = totalDistance / (gm.subjects.Count - 1);

            if (distanceAvg < focusThreshold)
            {
                result.Add(currentTrans);
            }
        }

        return result.ToArray();
    }

    Vector3 SolveTargetPosition()
    {

        if (tempBounds != null)
        {
            tempBounds.center = Vector3.zero;
            tempBounds.size = Vector3.zero;
        }

        // Solving for singular target
        if (subjectsInFocus.Length == 1)
        {
            // Disable auto zoom
            autoZoom = false;

            return subjectsInFocus[0].position + targetOffset;
        }

        // Solving for dual targets
        else if (subjectsInFocus.Length == 2)
        {
            // Enable auto zoom
            autoZoom = true;

            // Calculate middle position
            Vector3 midPoint = Vector3.Lerp(subjectsInFocus[0].position, subjectsInFocus[1].position, 0.5f);

            return midPoint + targetOffset;
        }

        // Solving for multiple targets
        else if (subjectsInFocus.Length >= 3)
        {
            // Enable auto zoom
            autoZoom = true;

            tempBounds = new Bounds(subjectsInFocus[0].position, Vector3.zero);

            for (int i = 1; i < subjectsInFocus.Length; i++)
            {
                tempBounds.Encapsulate(subjectsInFocus[i].position);
            }
            
            return tempBounds.center + targetOffset;
        }

        return Vector3.zero;
    }

    Vector3 CalculateCameraPosition(Vector3 target)
    {
        // Calculate hypotenuse
        float hyp = cameraDistance;

        // Picking the lowest FOV (height vs width) according to the screen resolution
        // Lowest FOV wins
        float lowestFOV = 0.0f;
        
        if (Camera.main.fieldOfView < CalculateFOVHeightFromFOVWidth(Camera.main.fieldOfView) * 0.5f)
        {
            lowestFOV = Camera.main.fieldOfView;
        }
        else
        {
            lowestFOV = CalculateFOVHeightFromFOVWidth(Camera.main.fieldOfView) * 0.5f;
        }

        // Only for multi subject situations
        if (autoZoom)
        {
            // Calculate zoom to accommodate 2 subjects
            if (subjectsInFocus.Length == 2)
            {
                // Distance between both subjects
                float distBetween = Vector3.Distance(subjectsInFocus[0].position, subjectsInFocus[1].position) + zoomPadding;

                hyp = (distBetween * 0.5f) * Mathf.Tan(lowestFOV * Mathf.Deg2Rad);
                hyp += cameraDistance;
            }

            // Calculate zoom to accommodate more than 2 subjects
            else
            {
                if (tempBounds != null)
                {
                    float maxDistance = Vector3.Distance(tempBounds.center, tempBounds.max) * 2 + zoomPadding;

                    hyp = (maxDistance * 0.5f) * Mathf.Tan(lowestFOV * Mathf.Deg2Rad);
                    hyp += cameraDistance;
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
