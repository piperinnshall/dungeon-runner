using UnityEngine;

public class Skeleton_Behaviour : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    public EnemyState currentState = EnemyState.Patrol;

    private Skeleton_PatrolState patrolState;
    private Skeleton_ChaseState chaseState;
    private Skeleton_AttackState attackState;

    void Start()
    {
        patrolState = GetComponent<Skeleton_PatrolState>();
        chaseState = GetComponent<Skeleton_ChaseState>();
        attackState = GetComponent<Skeleton_AttackState>();

        if (patrolState == null)
        {
            Debug.LogError("Skeleton_PatrolState is missing from the skeleton");
        }

        if (chaseState == null)
        {
            Debug.LogError("Skeleton_ChaseState is missing from the skeleton");
        }

        if (attackState == null)
        {
            Debug.LogError("Skeleton_AttackState is missing from the skeleton");
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                patrolState.UpdateState();
                break;

            case EnemyState.Chase:
                chaseState.UpdateState();
                break;

            case EnemyState.Attack:
                attackState.UpdateState();
                break;
        }
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        Debug.Log(gameObject.name + " changed state to " + currentState);
    }
}
