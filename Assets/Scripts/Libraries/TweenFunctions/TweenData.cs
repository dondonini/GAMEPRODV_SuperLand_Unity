using UnityEngine;

public class TweenData
{
    public bool modifyPosition;
    public bool modifyAngle;
    public bool modifyScale;

    public float duration;
    public bool isClamped;
    public EasingFunction.Ease func;
    public Transform actor; // huehuehue

    public Vector3 startPosition;
    public Quaternion startRotation;
    public Vector3 startScale;

    public Vector3 endPosition;
    public Quaternion endRotation;
    public Vector3 endScale;
};