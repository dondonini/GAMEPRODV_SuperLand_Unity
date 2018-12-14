using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingTurret_SM : MonoBehaviour
{
    /************************************************************************/
    /* Variables                                                            */
    /************************************************************************/

    [Header("State Settings")]
    [Header("General & Patrol")]
    public float maxMovementSpeed = 10.0f;
    public float movementAcceleration = 1.0f;
    public float movementDeceleration = 1.0f;
    public float turretLookRange = 10.0f;
    public float turrentLookFOV = 45.0f;

    [Header("Chase")]
    public float maxChaseSpeed = 20.0f;
    public float chaseDuration = 5.0f;
    
    [Header("Attack")]
    public float chargeDuration = 1.0f;
    public float firePause = 1.0f;

    [Header("Death")]
    public float deathPause = 3.0f;

    [Header("References")]
    public Transform rotationBody;
    public Transform projectilePrefab;
    public Transform projectileSpawner;
    public GameObject deathPoofPrefab;

    [Space]
    [ReadOnly]
    public Transform target;
    public Animator animator;
    public ParticleSystem particleSys;

    /************************************************************************/
    /* States                                                               */
    /************************************************************************/

    [HideInInspector]
    public AT_PatrolState   patrolState;
    [HideInInspector]
    public AT_ChaseState    chaseState;
    [HideInInspector]
    public AT_AttackState   attackState;
    [HideInInspector]
    public AT_AlertState    alertState;
    [HideInInspector]
    public AT_DeathState    deathState;

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/

    public EnemyStates_SM currentState;
    EnemyStates_SM previousState;
    [HideInInspector]
    public float rotationVelocity = 0.0f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(projectileSpawner.position, projectileSpawner.forward * turretLookRange);
    }

    void Awake()
    {
        patrolState = new AT_PatrolState(this);
        chaseState = new AT_ChaseState(this);
        attackState = new AT_AttackState(this);
        alertState = new AT_AlertState(this);
        deathState = new AT_DeathState(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = patrolState;

        SetCloseTarget();

        currentState.StartState();
    }

    // Update is called once per frame
    void Update()
    {
        // Update current state
        currentState.UpdateState();

        // Detect state change and call StartState method in current state
        if (previousState != null && previousState != currentState)
        {
            Debug.Log("State changed! " + previousState + " -> " + currentState);

            // Activate start state
            currentState.StartState();
        }

        // Update previous state
        previousState = currentState;
    }

    public void SetCloseTarget()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        float shortestDistance = float.MaxValue;
        GameObject closestPlayer = null;


        foreach (GameObject player in allPlayers)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < shortestDistance)
            {
                shortestDistance = Vector3.Distance(player.transform.position, transform.position);
                closestPlayer = player;
            }
        }

        if (closestPlayer)
            target = closestPlayer.transform;
        else
            Debug.LogError("There is not player in the game!");
    }

    public bool IsTargetInRange()
    {
        // Target doesn't exist
        if (!target) return false;

        float distance = Vector3.Distance(transform.position, target.position);

        // Target is out of range
        if (distance > turretLookRange) return false;

        // Target is in range
        return true;
    }

    public Vector3 GetTargetDirection()
    {
        if (!target) return Vector3.zero;

        return target.transform.position - transform.position;
    }

    #region Triggering

    public void OnTriggerEnter(Collider other)
    {
        // Interrupt for death sequence
        if (other.CompareTag("Player"))
            currentState = deathState;

        //currentState.OnTriggerEnter(other);
    }

    public void OnTriggerExit(Collider other)
    {
        currentState.OnTriggerExit(other);
    }

    #endregion
}
