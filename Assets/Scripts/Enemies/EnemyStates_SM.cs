using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyStates_SM
{
    void StartState();

    void UpdateState();

    void ToPatrolState();

    void ToAttackState();

    void ToChaseState();

    void ToWanderState();

    void OnTriggerEnter(Collider other);

    void OnTriggerExit(Collider other);
}
