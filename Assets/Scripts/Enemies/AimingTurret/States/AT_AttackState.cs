using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AT_AttackState : EnemyStates_SM
{
    private readonly AimingTurret_SM stateMachine;

    float elaspedTime = 0.0f;
    bool isFired = false;

    public AT_AttackState(AimingTurret_SM _SM)
    {
        stateMachine = _SM;
    }

    public void StartState()
    {
        // Reset values
        isFired = false;
        elaspedTime = 0.0f;

        // Start charge animation
        stateMachine.animator.SetBool("Charge/Fire", true);
    }

    public void UpdateState()
    {
        
        if (!isFired && elaspedTime >= stateMachine.chargeDuration)
        {
            // End of charge animation
            isFired = true;
            elaspedTime = 0.0f;

            // Start fire animation
            stateMachine.animator.SetBool("Charge/Fire", false);
        }
        else if (isFired && elaspedTime >= stateMachine.firePause)
        {
            // End of fire pause
            ToPatrolState();
        }

        elaspedTime += Time.deltaTime;
    }

    #region Transitions

    public void ToAttackState()
    {
        Debug.LogError("You cannot transition to current state!");
    }

    public void ToAlertState()
    {

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
