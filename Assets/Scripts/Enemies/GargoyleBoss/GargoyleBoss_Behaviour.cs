
using UnityEngine;

public class GargoyleBoss_Behaviour : MonoBehaviour
{
    public enum GargoyleBossState
    {
        Patrol,
        Chase,
        Attack
    }

    public bool isDead = false;
    public GargoyleBossState currentState = GargoyleBossState.Patrol;
    public float chaseRadius = 15f;

    private GargoyleBoss_PatrolState patrolState;
    private GargoyleBoss_ChaseState chaseState;
    private GargoyleBoss_AttackState attackState;

    void Start()
    {
        patrolState = GetComponent<GargoyleBoss_PatrolState>();
        chaseState = GetComponent<GargoyleBoss_ChaseState>();
        attackState = GetComponent<GargoyleBoss_AttackState>();

        if (patrolState == null)
            Debug.LogError("GargoyleBoss_PatrolState is missing from the GargoyleBoss");

        if (chaseState == null)
            Debug.LogError("GargoyleBoss_ChaseState is missing from the GargoyleBoss");

        if (attackState == null)
            Debug.LogError("GargoyleBoss_AttackState is missing from the GargoyleBoss");
    }

    void Update()
    {
        if (isDead)
            return;

        switch (currentState)
        {
            case GargoyleBossState.Patrol:
                patrolState.UpdateState();
                break;

            case GargoyleBossState.Chase:
                chaseState.UpdateState();
                break;

            case GargoyleBossState.Attack:
                attackState.UpdateState();
                break;
        }
    }

    public void ChangeState(GargoyleBossState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        Debug.Log(gameObject.name + " changed state to " + currentState);
    }
}
