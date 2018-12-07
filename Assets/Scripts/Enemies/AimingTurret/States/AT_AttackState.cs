using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AT_AttackState : EnemyStates_SM
{
    private readonly AimingTurret_SM stateMachine;

    public AT_AttackState(AimingTurret_SM _SM)
    {
        stateMachine = _SM;
    }

    public void StartState()
    {
        
    }

    public void UpdateState()
    {
        ToPatrolState();
    }

    #region Transitions

    public void ToAttackState()
    {
        Debug.LogError("You cannot transition to current state!");
    }

    public void ToChaseState()
    {
        
    }

    public void ToDeathState()
    {
        
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
