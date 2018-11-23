using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMovement : MonoBehaviour {
    
    [SerializeField]
    bool segmentEnabled = false;

    [SerializeField]
    Vector3 rootPosition;
    [SerializeField]
    float tweenInDuraction = 1.0f;
    [SerializeField]
    float tweenOutDuraction = 1.0f;
    [SerializeField]
    EasingFunction.Ease tweenEaseInFunction;
    [SerializeField]
    EasingFunction.Ease tweenEaseOutFunction;


    /************************************************************************/
    /* Cache                                                                */
    /************************************************************************/
    GameManager gameManager;
    TweenFunction tweenFunction;

    [SerializeField]
    Renderer thisRenderer;

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/
    [SerializeField]
    Vector3 targetPosition;
    Vector3 previousTargetPosition;
    Vector3 previousPosition;
    Coroutine currentTween;
    List<Transform> subjectChildren;

	// Use this for initialization
	void Start () {

        // Caching resources
        gameManager = GameManager.GetInstance();

        // Setting default variables
        rootPosition = transform.position;
        previousTargetPosition = transform.position;

        UpdateEnabled();

        transform.position = targetPosition;
    }
	
	// Update is called once per frame
	void Update () {

        UpdateEnabled();

        if (targetPosition != previousTargetPosition)
            StartCoroutine(StartTween());

        previousTargetPosition = targetPosition;
    }

    void UpdateEnabled()
    {
        if (!segmentEnabled)
        {
            targetPosition.y = gameManager.despawnHeight;

            // Remove any children from the subject list if any
            for (int c = 0; c < transform.childCount; c++)
            {
                for (int s = 0; s < gameManager.GetAllSubjectCount(); s++)
                {
                    if (transform.GetChild(c) == gameManager.GetAllSubjects()[s])
                    {
                        if (subjectChildren == null)
                            subjectChildren = new List<Transform>();

                        // Add subject into local cache
                        subjectChildren.Add(transform.GetChild(c).transform);

                        // Remove from public subject list
                        gameManager.RemoveSubject(transform.GetChild(c).transform);
                    }
                }
            }
        }
        else
        {
            targetPosition = rootPosition;

            // Add cached subject back to public subject list
            if (subjectChildren != null && subjectChildren.Count != 0)
            {
                for (int i = 0; i < subjectChildren.Count; i++)
                {
                    gameManager.AddSubject(subjectChildren[i]);
                    subjectChildren.RemoveAt(i);
                }
            }
        }
    }

    public void SegmentEnabled(bool val)
    {
        segmentEnabled = val;
    }

    public Vector3 GetRootPosition()
    {
        return rootPosition;
    }

    IEnumerator StartTween()
    {
        // Stop currently active tween if applicable
        if (currentTween != null)
        {
            StopCoroutine(currentTween);
            currentTween = null;
        }

        // Enable renderer is segment is enabled
        if (segmentEnabled)
            thisRenderer.enabled = true;

        // Cache previous position for reference
        previousPosition = transform.position;

        // Caching coroutine for future use
        currentTween = StartCoroutine(UpdatePosition((result) =>
        {
            // Callback code
            if (result)

                // Disable renderer if segment is disabled
                if (!segmentEnabled)
                    thisRenderer.enabled = false;
        }));

        yield return null;
    }

    /// <summary>
    /// Position tweening animation
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    IEnumerator UpdatePosition(System.Action<bool> callback)
    {
        float tweenDuration;
        EasingFunction.Ease tweenFunction;

        if (segmentEnabled)
        {
            tweenDuration = tweenInDuraction;
            tweenFunction = tweenEaseInFunction;
        }
        else
        {
            tweenDuration = tweenOutDuraction;
            tweenFunction = tweenEaseOutFunction;
        }

        for (float elaspedTime = 0.0f; elaspedTime <= tweenDuration; elaspedTime += Time.deltaTime)
        {
            float progress = elaspedTime / tweenDuration;

            float easeExpression;

            easeExpression = EasingFunction.GetEasingFunction(tweenFunction)(0.0f, 1.0f, progress);

            transform.position = Vector3.LerpUnclamped(previousPosition, targetPosition, easeExpression);

            yield return null;
        }

        transform.position = targetPosition;

        yield return null;
        callback(true);
    }
}
