using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMovement : MonoBehaviour {

    [SerializeField]
    Vector3 targetPosition;
    [SerializeField]
    float tweenDuraction = 1.0f;
    [SerializeField]
    EasingFunction.Ease tweenEaseFunction;

    /************************************************************************/
    /* Cache                                                                */
    /************************************************************************/
    TweenFunction tweenFunction;

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/
    Vector3 previousTargetPosition;
    Vector3 previousPosition;
    bool tweenActive = false;

	// Use this for initialization
	void Start () {


        // Setting default variables
        
        previousTargetPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if (targetPosition != previousTargetPosition)
        {
            StartTween();
        }

        previousTargetPosition = targetPosition;
    }

    void StartTween()
    {
        previousPosition = transform.position;

        StartCoroutine(UpdatePosition());
    }

    IEnumerator UpdatePosition()
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
    }
}
