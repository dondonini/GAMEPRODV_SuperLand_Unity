using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AT_AlertState : EnemyStates_SM
{
    private readonly AimingTurret_SM stateMachine;

    float animationDuration = 0.833f;

    float elaspedTime = 0.0f;

    // Constructor (Awake)
    public AT_AlertState(AimingTurret_SM _SM)
    {
        stateMachine = _SM;
    }

    // Start is called before the first frame update
    public void StartState()
    {
        elaspedTime = 0.0f;
        stateMachine.animator.SetTrigger("Alerted");
    }

    // Update is called once per frame
    public void UpdateState()
    {
        if (elaspedTime >= animationDuration)
        {
            ToChaseState();
        }

        elaspedTime += Time.deltaTime;
    }

    // Transitions
    #region Transitions

    public void ToPatrolState()
    {
        
    }

    public void ToAttackState()
    {

    }

    public void ToAlertState()
    {
        Debug.LogError("You cannot transition to current state!");
    }

    public void ToChaseState()
    {
        stateMachine.currentState = stateMachine.chaseState;
    }

    public void ToDeathState()
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
