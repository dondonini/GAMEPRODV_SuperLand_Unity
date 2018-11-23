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
        if (isGoingClockwise && stateMachine.rotationVelocity < stateMachine.maxMovementSpeed)
        {
            stateMachine.rotationVelocity = stateMachine.rotationVelocity + stateMachine.movementAcceleration * Time.deltaTime;
        }
        else if (!isGoingClockwise && stateMachine.rotationVelocity < -stateMachine.maxMovementSpeed)
        {
            stateMachine.rotationVelocity = stateMachine.rotationVelocity - stateMachine.movementAcceleration * Time.deltaTime;
        }

        // Apply rotation to body
        stateMachine.rotationBody.Rotate(new Vector3(0.0f, stateMachine.rotationVelocity * Time.deltaTime, 0.0f));
    }

    // Transitions
    #region Transitions

    public void ToPatrolState()
    {
        // Cannot transition to self
    }

    public void ToAttackState()
    {

    }

    public void ToChaseState()
    {

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
