using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AT_PatrolState : EnemyStates_SM
{
    private readonly AimingTurret_SM stateMachine;

    // Runtime Variables

    bool isGoingClockwise = false;

    // Constructor (Awake)
    public AT_PatrolState(AimingTurret_SM _SM)
    {
        stateMachine = _SM;
    }

    // Start is called before the first frame update
    public void StartState()
    {
        // Determine which direction the turret is going to spin relative to past velocity;
        isGoingClockwise = stateMachine.rotationVelocity > 0.0f;
    }

    // Update is called once per frame
    public void UpdateState()
    {
        // Rotate
        if (isGoingClockwise)
        {
            stateMachine.rotationVelocity = stateMachine.rotationVelocity + stateMachine.movementAcceleration * Time.deltaTime;
        }
        else //(!isGoingClockwise && stateMachine.rotationVelocity < -stateMachine.maxMovementSpeed)
        {
            stateMachine.rotationVelocity = stateMachine.rotationVelocity - stateMachine.movementAcceleration * Time.deltaTime;
        }

        stateMachine.rotationVelocity = Mathf.Clamp(stateMachine.rotationVelocity, -stateMachine.maxMovementSpeed, stateMachine.maxMovementSpeed);

        // Apply rotation to body
        stateMachine.rotationBody.Rotate(new Vector3(0.0f, stateMachine.rotationVelocity, 0.0f));

        CheckTargetIsInFOV();
    }

    void CheckTargetIsInFOV()
    {
        if (!stateMachine.IsTargetInRange()) return;

        Vector3 targetDir = stateMachine.GetTargetDirection();
        float angleToTarget = Vector3.Angle(targetDir, stateMachine.rotationBody.forward);

        // Check if target is in FOV
        if (angleToTarget <= stateMachine.turrentLookFOV * 0.5f && angleToTarget >= -stateMachine.turrentLookFOV * 0.5f)
        {
            ToChaseState();
        }
    }

    // Transitions
    #region Transitions

    public void ToPatrolState()
    {
        Debug.LogError("You cannot transition to current state!");
    }

    public void ToAttackState()
    {

    }

    public void ToChaseState()
    {
        stateMachine.currentState = stateMachine.chaseState;
    }

    public void ToDeathState()
    {
        throw new System.NotImplementedException();
    }

    public void ToWanderState()
    {

    }

    #endregion

    public void OnTriggerEnter(Collider other)
    {

    }

    public void OnTriggerExit(Collider other)
    {

    }
}
