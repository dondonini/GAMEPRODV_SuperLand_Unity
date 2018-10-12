﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMovement : MonoBehaviour {
    
    [SerializeField]
    bool segmentEnabled = false;

    [SerializeField]
    Vector3 rootPosition;
    [SerializeField]
    float tweenDuraction = 1.0f;
    [SerializeField]
    EasingFunction.Ease tweenEaseFunction;


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
            targetPosition.y = gameManager.WORLD_SPAWN_HEIGHT;
        else
            targetPosition = rootPosition;
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
        for (float elaspedTime = 0.0f; elaspedTime <= tweenDuraction; elaspedTime += Time.deltaTime)
        {
            float progress = elaspedTime / tweenDuraction;

            float easeExpression = EasingFunction.GetEasingFunction(tweenEaseFunction)(0.0f, 1.0f, progress);

            transform.position = Vector3.LerpUnclamped(previousPosition, targetPosition, easeExpression);

            yield return null;
        }

        transform.position = targetPosition;

        yield return null;
        callback(true);
    }
}
