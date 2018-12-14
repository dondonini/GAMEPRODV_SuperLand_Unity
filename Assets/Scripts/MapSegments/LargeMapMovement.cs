using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeMapMovement : MonoBehaviour, IMapMove {
    
    [Tooltip("Ignores manager instructions")]
    public bool ignoreManager = false;

    [SerializeField]
    bool segmentEnabled = false;

    [Header("Positioning")]
    [SerializeField]
    Vector3 rootPosition;
    [SerializeField]
    Vector3 targetPosition;

    [Header("Attachments")]
    [SerializeField]
    Transform[] attachedGameObjects;

    [Header("Animation Properties")]
    [SerializeField]
    float tweenInDuraction = 1.0f;
    [SerializeField]
    float tweenOutDuraction = 1.0f;
    [SerializeField]
    EasingFunction.Ease tweenEaseInFunction;
    [SerializeField]
    EasingFunction.Ease tweenEaseOutFunction;

    [Header("References")]

    /************************************************************************/
    /* Cache                                                                */
    /************************************************************************/
    GameManager gameManager;
    TweenFunction tweenFunction;

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/
    Vector3 previousTargetPosition;
    Vector3 previousPosition;
    Coroutine currentTween;
    List<Transform> subjectChildren;

    Bounds finalBounds;

    private void OnDrawGizmosSelected()
    {
        // Make bound box for all children
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(GetAbsolutePosition(), GetAbsoluteScale());
    }

    private void Awake()
    {
        AttachGameObjects();
    }

    // Use this for initialization
    void Start () {

        // Grab resources
        gameManager = GameManager.GetInstance();

        // Attach gameobjects to segment
        UpdateEnabled();

        // Setting default variables
        finalBounds = new Bounds();

        rootPosition = transform.position;
        targetPosition = rootPosition;

        Vector3 startingPos = targetPosition;
        startingPos.y = gameManager.despawnHeight;
        transform.position = startingPos;

        previousPosition = startingPos;

        StartCoroutine(StartTween());
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
        if (segmentEnabled)
        {
            EnableSegment();
        }
        else
        {
            DisableSegment();   
        }
    }

    void AttachGameObjects()
    {
        for (int a = 0; a < attachedGameObjects.Length; a++)
        {
            attachedGameObjects[a].SetParent(transform);
        }
    }

    void DisableSegment()
    {
        targetPosition = rootPosition;
        targetPosition.y = gameManager.despawnHeight;
    }

    void DisableAttachments()
    {
        // Remove any children from the subject list if any
        for (int c = 0; c < transform.childCount; c++)
        {
            // Skip mesh from being scanned
            if (transform.GetChild(c).name == "Mesh") continue;

            for (int s = 0; s < gameManager.GetAllSubjectCount(); s++)
            {
                if (transform.GetChild(c) == gameManager.allSubjects[s])
                {
                    if (subjectChildren == null)
                        subjectChildren = new List<Transform>();

                    // Add subject into local cache
                    subjectChildren.Add(transform.GetChild(c).transform);

                    // Remove from public subject list
                    gameManager.RemoveSubject(subjectChildren[0]);

                    // Deactivate subject
                    subjectChildren[0].gameObject.SetActive(false);
                }
            }
        }
    }

    void EnableSegment()
    {
        targetPosition = rootPosition;
    }

    void EnableAttachments()
    {
        // Add cached subject back to public subject list
        if (subjectChildren != null && subjectChildren.Count != 0)
        {
            for (int i = 0; i < subjectChildren.Count; i++)
            {
                // Activate subject
                subjectChildren[i].gameObject.SetActive(true);

                // Add subject into global subject list
                gameManager.AddSubject(subjectChildren[i]);

                // Remove from cache
                subjectChildren.RemoveAt(i);
            }
        }
    }

    public void SegmentEnabled(bool val)
    {
        segmentEnabled = val;
    }

    public bool IsSegmentEnabled()
    {
        return segmentEnabled;
    }

    public bool IsIgnored()
    {
        return ignoreManager;
    }

    public Vector3 GetRootPosition()
    {
        return rootPosition;
    }

    public Vector3 GetAbsoluteScale()
    {
        // Recursivly get all renderers in children
        Renderer[] allChildrenRenderers = transform.GetComponentsInChildren<Renderer>();

        // Encapsulate all renderer bounds into one bound
        Bounds segmentBounds = new Bounds();
        for (int r = 0; r < allChildrenRenderers.Length; r++)
        {
            Renderer selectedRenderer = allChildrenRenderers[r];

            segmentBounds.Encapsulate(selectedRenderer.bounds);
        }

        if (finalBounds.size.x > segmentBounds.size.x ||
            finalBounds.size.y > segmentBounds.size.y ||
            finalBounds.size.z > segmentBounds.size.z)
        {
            return finalBounds.size;
        }
        else
        {
            finalBounds.size = segmentBounds.size;

            // Return results
            return segmentBounds.size;
        }

        
    }

    public Vector3 GetAbsolutePosition()
    {
        // Recursivly get all transforms in children
        Transform[] allChildrenTransforms = transform.GetComponentsInChildren<Transform>();

        // Add all positions together
        Vector3 totalPositions = Vector3.zero;

        for (int t = 0; t < allChildrenTransforms.Length; t++)
        {
            Transform selectedTransform = allChildrenTransforms[t];

            totalPositions += selectedTransform.position;
        }

        // Calculate average position
        totalPositions /= allChildrenTransforms.Length;

        return totalPositions;
    }

    public Bounds GetSegmentBounds()
    {
        return new Bounds(GetAbsolutePosition(), GetAbsoluteScale());
    }

    void DisableAllChildren()
    {
        if (transform.childCount == 0) return;

        for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
        {
            Transform selectedChild = transform.GetChild(childIndex);

            selectedChild.gameObject.SetActive(false);
        }
    }

    void ActivateAllChildren()
    {
        if (transform.childCount == 0) return;

        for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
        {
            Transform selectedChild = transform.GetChild(childIndex);

            selectedChild.gameObject.SetActive(true);
        }
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
        { 
            ActivateAllChildren();
            EnableAttachments();
        }

        // Cache previous position for reference
        previousPosition = transform.position;

        // Caching coroutine for future use
        currentTween = StartCoroutine(UpdatePosition((result) =>
        {
            // Callback code
            if (result)

                // Disable renderer if segment is disabled
                if (!segmentEnabled)
                { 
                    DisableAllChildren();
                    DisableAttachments();
                }
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
