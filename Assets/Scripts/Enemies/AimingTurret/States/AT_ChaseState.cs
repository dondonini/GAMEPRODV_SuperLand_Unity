using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AT_ChaseState : EnemyStates_SM
{
    private readonly AimingTurret_SM stateMachine;

    float chaseTime = 0.0f;

    public AT_ChaseState(AimingTurret_SM _SM)
    {
        stateMachine = _SM;
    }

    // Start is called before the first frame update
    public void StartState()
    {
        // Reset chase timer
        chaseTime = 0.0f;
    }

    // Update is called once per frame
    public void UpdateState()
    {
        // Return to patrol state if target is out of range
        if (!stateMachine.IsTargetInRange()) ToPatrolState();

        // Get the direction from self to target
        Vector3 targetDir = stateMachine.GetTargetDirection();

        // Calculate movement step speed 
        float step = stateMachine.maxChaseSpeed * Time.deltaTime;

        // Calculate new look rotation
        Vector3 newDir = Vector3.RotateTowards(stateMachine.rotationBody.forward, targetDir, step, 0.0f);

        stateMachine.rotationBody.rotation = Quaternion.LookRotation(newDir);
        
        // Reset X and Z axis
        Vector3 tempEulerAngles = stateMachine.rotationBody.eulerAngles;
        tempEulerAngles.x = 0.0f;
        tempEulerAngles.z = 0.0f;
        stateMachine.rotationBody.eulerAngles = tempEulerAngles;

        // Attack after timer
        if (chaseTime >= stateMachine.chaseDuration)
        {
            ToAttackState();
        }

        // Increment timer
        chaseTime += Time.deltaTime;
    }

    #region Transitions

    public void ToAttackState()
    {
        stateMachine.currentState = stateMachine.attackState;
    }

    public void ToChaseState()
    {
        Debug.LogError("You cannot transition to current state!");
    }

    public void ToDeathState()
    {
        throw new System.NotImplementedException();
    }

    public void ToPatrolState()
    {
        stateMachine.currentState = stateMachine.patrolState;
    }

    public void ToWanderState()
    {
        
    }

    #endregion

    #region Triggers

    public void OnTriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public void OnTriggerExit(Collider other)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
