using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AT_DeathState : EnemyStates_SM
{
    private readonly AimingTurret_SM stateMachine;

    float elaspedTime = 0.0f;

    public AT_DeathState(AimingTurret_SM _sm)
    {
        stateMachine = _sm;
    }

    public void StartState()
    {
        stateMachine.animator.SetTrigger("Death");
        stateMachine.particleSys.Play();
    }

    public void UpdateState()
    {
        if (elaspedTime >= stateMachine.deathPause)
        {
            // Death poof!
            GameObject newPoof = Object.Instantiate(stateMachine.deathPoofPrefab) as GameObject;

            newPoof.transform.position = stateMachine.transform.position;

            // Destroy self!
            Object.Destroy(stateMachine.gameObject);
        }

        elaspedTime += Time.deltaTime;
    }

    #region Transitions

    public void ToAlertState()
    {
        
    }

    public void ToAttackState()
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
        
    }

    public void ToWanderState()
    {
        
    }

    #endregion

    #region Triggers

    public void OnTriggerEnter(Collider other)
    {
        
    }

    public void OnTriggerExit(Collider other)
    {
        
    }

    #endregion
}
