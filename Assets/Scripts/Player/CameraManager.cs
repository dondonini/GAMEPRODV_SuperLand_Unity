using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public float cameraDistance = 100.0f;
    public float cameraAngleHeight = 30.0f;
    public float cameraAngleRotation = 45.0f;
    public float speedDamp = 0.001f;
    public Vector3 positionOffset = Vector3.zero;
    public Transform mainCamera;
    public Transform cameraAim;

    private GameManager gm;

    private Vector3 mainCameraV = Vector3.zero;
    private Vector3 cameraAimV = Vector3.zero;
    private Bounds tempBounds;

	// Use this for initialization
	void Start () {
        // Caching
		gm = GameManager.GetInstance();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		mainCamera.transform.localPosition = CalculateCameraPosition(Vector3.zero);
        
        Vector3 targetPosition = SolveTargetPosition();

        // SmoothDamp rig position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref mainCameraV, speedDamp);

        cameraAim.position = Vector3.SmoothDamp(cameraAim.position, targetPosition, ref cameraAimV, speedDamp);
	}

    Vector3 SolveTargetPosition()
    {
        // Solving for singular target
        if (gm.subjects.Count == 1)
        {
            return gm.subjects[0].position;
        }

        // Solving for dual targets
        else if (gm.subjects.Count == 2)
        {
            // Calculate middle position
            Vector3 midPoint = Vector3.Lerp(gm.subjects[0].position, gm.subjects[1].position, 0.5f);

            return midPoint;
        }

        // Solving for multiple targets
        else if (gm.subjects.Count >= 3)
        {
            if (tempBounds == null)
            {
                tempBounds = new Bounds(gm.subjects[0].position, gm.subjects[1].position);
            }

            for (int i = 2; i < gm.subjects.Count; i++)
            {
                tempBounds.Encapsulate(gm.subjects[i].position);
            }

            return tempBounds.center;
        }
        else
        {
            return Vector3.zero;
        }
    }

    Vector3 CalculateCameraPosition(Vector3 target)
    {
        // Calculate adjacent
        float adj = Mathf.Cos(cameraAngleHeight) * Mathf.Sqrt(cameraDistance);

        // Calculate opposite
        float opp = Mathf.Pow(cameraDistance,2) - Mathf.Pow(adj, 2);

        // Solving camera position
        Vector3 newPosition = target;

        newPosition.y += opp;
        newPosition.z += adj;

        return newPosition;
    }
}
