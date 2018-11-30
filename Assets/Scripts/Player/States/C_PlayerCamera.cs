using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PlayerCamera : CameraStates_SM
{

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/

    private CameraManager manager;
    private Vector3 rigV = Vector3.zero;
    private Vector3 mainCameraV = Vector3.zero;
    private Vector3 cameraAimV = Vector3.zero;
    private float rigRotationV = 0.0f;
    private Vector2 previousRotationAdjust = Vector2.zero;
    private Bounds tempBounds;
    private Transform[] subjectsInFocus;
    private float dynamicMovementDamp;
    private float dynamicRotationDamp;

    // This only enables if there are more than one subject in the shot
    private bool autoZoom = false;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="_man"></param>
    public C_PlayerCamera(CameraManager _man)
    {
        manager = _man;
    }

    // Use this for initialization
    public void StartState()
    {
        // Caching
        dynamicMovementDamp = manager.cameraMovementDamp;
        dynamicRotationDamp = manager.cameraRotationDamp;

    }

    public void UpdateState()
    {
        UpdateInput();
    }

    // Update is called once per frame
    public void LateUpdateState()
    {

        subjectsInFocus = UpdateSubjectsInFocus();

        Vector3 targetPosition = SolveTargetPosition();

        // SmoothDamp rig position
        manager.transform.position = Vector3.SmoothDamp(manager.transform.position, targetPosition, ref mainCameraV, dynamicMovementDamp);

        // SmoothDamp camera position
        manager.mainCamera.transform.localPosition = Vector3.SmoothDamp(manager.mainCamera.transform.localPosition, CalculateCameraPosition(Vector3.zero), ref rigV, dynamicRotationDamp);

        // SmoothDamp target position
        manager.cameraAim.position = Vector3.SmoothDamp(manager.cameraAim.position, targetPosition, ref cameraAimV, dynamicMovementDamp);

        // SmoothDamp rig rotation
        Vector3 rigEulerAngle = manager.transform.eulerAngles;
        rigEulerAngle.y = Mathf.SmoothDampAngle(rigEulerAngle.y, manager.cameraAngleRotation, ref rigRotationV, dynamicRotationDamp);
        manager.transform.eulerAngles = rigEulerAngle;

        // Aim camera at smooth target
        manager.mainCamera.transform.LookAt(manager.cameraAim);

        // Sync main camera positon and rotation to GUICamera
        manager.guiCamera.transform.position = manager.mainCamera.transform.position;
        manager.guiCamera.transform.rotation = manager.mainCamera.transform.rotation;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!manager.showDebug) return;
        if (!Application.isPlaying) return;

        // Camera to target smooth
        Gizmos.color = Color.red;
        Gizmos.DrawLine(manager.mainCamera.transform.position, manager.cameraAim.position);
        Gizmos.DrawCube(manager.cameraAim.position, new Vector3(0.1f, 0.1f, 0.1f));

        // Camera to target target positions
        Gizmos.color = Color.green;
        Gizmos.DrawLine(CalculateCameraPosition(SolveTargetPosition()), SolveTargetPosition());
        Gizmos.DrawCube(SolveTargetPosition(), new Vector3(0.1f, 0.1f, 0.1f));
        Gizmos.DrawCube(CalculateCameraPosition(SolveTargetPosition()), new Vector3(0.1f, 0.1f, 0.1f));

        // Target visuals
        Gizmos.color = Color.blue;
        if (subjectsInFocus.Length == 1)
        {
            Gizmos.DrawCube(subjectsInFocus[0].position, new Vector3(0.1f, 0.1f, 0.1f));
        }
        else if (subjectsInFocus.Length == 2)
        {
            Transform sub1 = subjectsInFocus[0];
            Transform sub2 = subjectsInFocus[1];

            Gizmos.DrawLine(sub1.position, sub2.position);
            Gizmos.DrawCube(sub1.position, new Vector3(0.1f, 0.1f, 0.1f));
            Gizmos.DrawCube(sub2.position, new Vector3(0.1f, 0.1f, 0.1f));
        }
        else if (subjectsInFocus.Length >= 3)
        {
            Gizmos.DrawWireCube(tempBounds.center, tempBounds.size);
            for (int i = 0; i < subjectsInFocus.Length; i++)
            {
                Gizmos.DrawCube(subjectsInFocus[i].position, new Vector3(0.1f, 0.1f, 0.1f));
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
                AdjustCameraRotation(new Vector2(Input.GetAxis("Mouse X"), 0.0f));
            }

            if (Input.GetAxis("Mouse Y") != 0.0f)
            {
                AdjustCameraRotation(new Vector2(0.0f, Input.GetAxis("Mouse Y")));
            }
        }

        /************************************************************************/
        /* Camera Zoom                                                          */
        /************************************************************************/

        if (Input.GetAxis("Mouse ScrollWheel") != 0.0f)
            AdjustCameraZoom(Input.GetAxis("Mouse ScrollWheel"));

        //////////////////////////////////////////////////////////////////////////
        /// Controller
        /// 

        /************************************************************************/
        /* Camera Rotation                                                      */
        /************************************************************************/

        if (Input.GetAxis("Player1_RightStickX") != 0.0f)
        {
            AdjustCameraRotation(new Vector2(Input.GetAxis("Player1_RightStickX"), 0.0f));
        }

        if (Input.GetAxis("Player1_RightStickY") != 0.0f)
        {
            AdjustCameraRotation(new Vector2(0.0f, Input.GetAxis("Player1_RightStickY")));
        }
    }

    void AdjustCameraRotation(Vector2 _adjustment)
    {
        manager.cameraAngleRotation += _adjustment.x;
        manager.cameraAngleHeight = Mathf.Clamp(manager.cameraAngleHeight - _adjustment.y, 0.0f, 80.0f);

        float fastestMovement = 1.0f;

        if (_adjustment.x > fastestMovement)
        {
            dynamicMovementDamp = 0.1f;
        }
        else
        {
            dynamicMovementDamp = manager.cameraMovementDamp;
        }

        if (_adjustment.y > fastestMovement)
        {
            dynamicRotationDamp = 0.1f;
        }
        else
        {
            dynamicRotationDamp = manager.cameraRotationDamp;
        }

        if (manager.cameraAngleRotation > 360.0f)
        {
            manager.cameraAngleRotation = 0.0f;
        }
        else if (manager.cameraAngleRotation < 0.0f)
        {
            manager.cameraAngleRotation = 360.0f;
        }
    }

    void AdjustCameraZoom(float _adjustment)
    {
        manager.cameraDistance = Mathf.Clamp(manager.cameraDistance - _adjustment, manager.minZoom, manager.maxZoom);
    }

    Transform[] UpdateSubjectsInFocus()
    {
        // Single subject
        if (manager.gm.GetAllSubjectCount() == 1)
        {
            return manager.gm.GetAllSubjects();
        }

        // 2 subjects
        if (manager.gm.GetAllSubjectCount() == 2)
        {
            if (Vector3.Distance(manager.gm.GetAllSubjects()[0].position, manager.gm.GetAllSubjects()[1].position) > manager.focusThreshold)
            {
                return new Transform[] { manager.gm.GetMainSubject() };
            }
            else
            {
                return manager.gm.GetAllSubjects();
            }
        }

        // More than 2 subjects
        List<Transform> result = new List<Transform>();

        for (int s = 0; s < manager.gm.GetAllSubjectCount(); s++)
        {
            float totalDistance = 0;
            Transform currentTrans = manager.gm.GetAllSubjects()[s];

            for (int other = 0; other < manager.gm.GetAllSubjectCount(); other++)
            {
                if (currentTrans == manager.gm.GetAllSubjects()[other])
                    continue;

                float distanceFrom = Vector3.Distance(currentTrans.position, manager.gm.GetAllSubjects()[other].position);

                totalDistance += distanceFrom;
            }

            float distanceAvg = totalDistance / (manager.gm.GetAllSubjectCount() - 1);

            if (distanceAvg < manager.focusThreshold)
            {
                result.Add(currentTrans);
            }
            else if (currentTrans == manager.gm.GetMainSubject())
            {
                return new Transform[] { currentTrans };
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

            return subjectsInFocus[0].position + manager.targetOffset;
        }

        // Solving for dual targets
        else if (subjectsInFocus.Length == 2)
        {
            // Enable auto zoom
            autoZoom = true;

            // Calculate middle position
            Vector3 midPoint = Vector3.Lerp(subjectsInFocus[0].position, subjectsInFocus[1].position, Mathf.Clamp01(manager.cameraDistance - manager.minZoom / manager.zoomTransition) * 0.5f);

            return midPoint + manager.targetOffset;
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

            // Set micro zoom
            Vector3 midPoint = Vector3.Lerp(subjectsInFocus[0].position, (tempBounds.center + manager.targetOffset), Mathf.Clamp01(manager.cameraDistance - manager.minZoom / manager.zoomTransition) * 0.5f);

            return midPoint;
        }

        return Vector3.zero;
    }

    Vector3 CalculateCameraPosition(Vector3 target)
    {
        // Calculate hypotenuse
        float hyp = manager.cameraDistance;

        // Picking the lowest FOV (height vs width) according to the screen resolution
        // Lowest FOV wins
        float lowestFOV = 0.0f;

        if (Camera.main.fieldOfView < manager.CalculateFOVHeightFromFOVWidth(Camera.main.fieldOfView) * 0.5f)
        {
            lowestFOV = Camera.main.fieldOfView;
        }
        else
        {
            lowestFOV = manager.CalculateFOVHeightFromFOVWidth(Camera.main.fieldOfView) * 0.5f;
        }

        // Only for multi subject situations
        if (autoZoom)
        {
            // Calculate zoom to accommodate 2 subjects
            if (subjectsInFocus.Length == 2)
            {
                // Distance between both subjects
                float distBetween = Vector3.Distance(subjectsInFocus[0].position, subjectsInFocus[1].position) + manager.zoomPadding;

                hyp = (distBetween * 0.5f) * Mathf.Tan(lowestFOV * Mathf.Deg2Rad);
            }

            // Calculate zoom to accommodate more than 2 subjects
            else
            {
                if (tempBounds != null)
                {
                    float maxDistance = Vector3.Distance(tempBounds.center, tempBounds.max) * 2 + manager.zoomPadding;

                    hyp = (maxDistance * 0.5f) * Mathf.Tan(lowestFOV * Mathf.Deg2Rad);
                }
            }
        }

        // Active zoom
        if (manager.cameraDistance < manager.zoomTransition)
        {
            // Zoom all the way
            hyp = hyp * (manager.cameraDistance / manager.zoomTransition);
        }
        else
        {
            hyp += manager.cameraDistance;
        }

        // Calculate adjacent
        float adj = hyp * Mathf.Cos(manager.cameraAngleHeight * Mathf.Deg2Rad);

        // Calculate opposite
        float opp = Mathf.Sqrt(Mathf.Pow(hyp, 2) - Mathf.Pow(adj, 2));

        // Solving camera position
        Vector3 newPosition = target;

        newPosition.y += opp;
        newPosition.z -= adj;

        return newPosition;
    }

    
}
