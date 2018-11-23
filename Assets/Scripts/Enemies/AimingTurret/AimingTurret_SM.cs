using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingTurret_SM : MonoBehaviour
{
    // Variables
    public float maxMovementSpeed = 10.0f;
    public float movementAcceleration = 1.0f;
    public float movementDeceleration = 1.0f;

    // References
    public Transform rotationBody;

    // States

    [HideInInspector]
    public AT_PatrolState patrolState;

    // Runtime Variables

    EnemyStates_SM currentState;
    EnemyStates_SM previousState;
    [HideInInspector]
    public float rotationVelocity = 0.0f;

    void Awake()
    {
        patrolState = new AT_PatrolState(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Setup current state
        currentState = patrolState;
    }

    // Update is called once per frame
    void Update()
    {
        

        // DEBUG: Notifies you if state has changed
        if (previousState != null && previousState != currentState)
        {
            Debug.Log("State changed! " + previousState + " -> " + currentState);

            // Activates start state
            currentState.StartState();
        }

        // Update current state
        currentState.UpdateState();

        // Update previous state
        previousState = currentState;
    }

    // Collisions

    public void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }

    public void OnTriggerExit(Collider other)
    {
        currentState.OnTriggerExit(other);
    }
}
