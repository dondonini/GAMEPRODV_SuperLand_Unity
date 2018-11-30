using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    [Header("Global Variables")]
    [Range(0.1f, 100.0f)]
    public float cameraDistance = 10.0f;
    public float minZoom = 2.0f;
    public float maxZoom = 100.0f;
    [Tooltip("The transition point where the camera starts zooming into the main subject")]
    public float zoomTransition = 10.0f;
    [Range(0, 180)]
    public float cameraAngleHeight = 30.0f;
    public float cameraAngleRotation = 45.0f;
    public float cameraMovementDamp = 1.0f;
    public float cameraRotationDamp = 0.2f;
    public Vector3 targetOffset = Vector3.zero;
    public float zoomPadding = 20;
    public float focusThreshold = 50.0f;


    [Header("References")]
    public Camera mainCamera;
    public Camera guiCamera;
    public Transform cameraAim;
    public Transform guiScreenSpace;

    [Header("Debugging")]
    public bool showDebug = false;

    // States
    public C_PlayerCamera playerCState;

    CameraStates_SM currentState;
    CameraStates_SM previousState;

    [HideInInspector]
    public GameManager gm;

    private void OnValidate()
    {
        UpdateGUIScreenSpace();

        if (minZoom > cameraDistance)
        {
            minZoom = cameraDistance;
        }

        zoomTransition = Mathf.Clamp(zoomTransition, minZoom, maxZoom);
    }

    private void Awake()
    {
        playerCState = new C_PlayerCamera(this);
    }

    // Use this for initialization
    void Start () {
        // Caching
		gm = GameManager.GetInstance();

        currentState = playerCState;

    }

    private void Update()
    {
        // DEBUG: Notifies you if state has changed
        if (previousState != null && previousState != currentState)
        {
            Debug.Log("State changed! " + previousState + " -> " + currentState);

            // Activates start state
            currentState.StartState();
        }

        // Update current state
        currentState.UpdateState();

        // Update previous state
        previousState = currentState;
    }

    // Update is called once per frame
    void LateUpdate () 
    {
        currentState.LateUpdateState();

        // Sync main camera positon and rotation to GUICamera
        guiCamera.transform.position = mainCamera.transform.position;
        guiCamera.transform.rotation = mainCamera.transform.rotation;
        UpdateGUIScreenSpace();
	}

    public Transform[] UpdateSubjectsInFocus()
    {
        // Single subject
        if (gm.GetAllSubjectCount() == 1)
        {
            return gm.GetAllSubjects();
        }

        // 2 subjects
        if (gm.GetAllSubjectCount() == 2)
        {
            if (Vector3.Distance(gm.GetAllSubjects()[0].position, gm.GetAllSubjects()[1].position) > focusThreshold)
            {
                return new Transform[]{gm.GetMainSubject() };
            }
            else
            {
                return gm.GetAllSubjects();
            }
        }

        // More than 2 subjects
        List<Transform> result = new List<Transform>();

        for (int s = 0; s < gm.GetAllSubjectCount(); s++)
        {
            float totalDistance = 0;
            Transform currentTrans = gm.GetAllSubjects()[s];

            for (int other = 0; other < gm.GetAllSubjectCount(); other++)
            {
                if (currentTrans == gm.GetAllSubjects()[other])
                    continue;

                float distanceFrom = Vector3.Distance(currentTrans.position, gm.GetAllSubjects()[other].position);
                
                totalDistance += distanceFrom;
            }

            float distanceAvg = totalDistance / (gm.GetAllSubjectCount() - 1);

            if (distanceAvg < focusThreshold)
            {
                result.Add(currentTrans);
            }
            else if (currentTrans == gm.GetMainSubject())
            {
                return new Transform[]{ currentTrans};
            }
        }

        return result.ToArray();
    }

    public float CalculateFOVHeightFromFOVWidth(float fovWidth)
    {
        
        // Calculate adjacent
        float adj = (GetScreenResolution().x * 0.5f) * Mathf.Tan((fovWidth * 0.5f) * Mathf.Deg2Rad);

        float fovHeight = (((GetScreenResolution().y * 0.5f) / adj) * Mathf.Rad2Deg) * 2.0f;

        return fovHeight;
    }

    public Vector2 GetScreenResolution()
    {
        return new Vector2(Screen.width, Screen.height);
    }

    void UpdateGUIScreenSpace()
    {
        float height = 2.0f * guiCamera.orthographicSize;
        float width = height * guiCamera.aspect;
        float depth = guiCamera.farClipPlane;

        guiScreenSpace.GetComponent<BoxCollider>().size = new Vector3(width, height, depth);
        guiScreenSpace.transform.localPosition = new Vector3(0.0f, 0.0f, depth * 0.5f);
    }
}
