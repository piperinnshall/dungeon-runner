using UnityEngine;

public class Gargoyle_Behaviour : MonoBehaviour
{
    public enum GargoyleState
    {
        Patrol,
        Chase,
        Attack
    }

    public bool isDead = false;
    public GargoyleState currentState = GargoyleState.Patrol;
    public float chaseRadius = 15f;
    public float staggerDuration = 1f;


    private Gargoyle_PatrolState patrolState;
    private Gargoyle_ChaseState chaseState;
    private Gargoyle_AttackState attackState;

    void Start()
    {
        patrolState = GetComponent<Gargoyle_PatrolState>();
        chaseState = GetComponent<Gargoyle_ChaseState>();
        attackState = GetComponent<Gargoyle_AttackState>();

        if (patrolState == null)
        {
            Debug.LogError("Gargoyle_PatrolState is missing from the Gargoyle");
        }

        if (chaseState == null)
        {
            Debug.LogError("Gargoyle_ChaseState is missing from the Gargoyle");
        }

        if (attackState == null)
        {
            Debug.LogError("Gargoyle_AttackState is missing from the Gargoyle");
        }
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        switch (currentState)
        {
            case GargoyleState.Patrol:
                patrolState.UpdateState();
                break;

            case GargoyleState.Chase:
                chaseState.UpdateState();
                break;

            case GargoyleState.Attack:
                attackState.UpdateState();
                break;
        }
    }

    public void ChangeState(GargoyleState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        Debug.Log(gameObject.name + " changed state to " + currentState);
    }

}
