using UnityEngine;

public class Skeleton_Behaviour : MonoBehaviour, EnemyStagger
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Stagger
    }
    public bool isDead = false;
    public EnemyState currentState = EnemyState.Patrol;
    public float chaseRadius = 20f;
    public float lastAttackTime = -10f;
    public float staggerDuration = 1.5f;
    private float staggerEndTime;

    private Skeleton_PatrolState patrolState;
    private Skeleton_ChaseState chaseState;
    private Skeleton_AttackState attackState;
    private Skeleton_StaggerState staggerState;

    void Start()
    {
        patrolState = GetComponent<Skeleton_PatrolState>();
        chaseState = GetComponent<Skeleton_ChaseState>();
        attackState = GetComponent<Skeleton_AttackState>();
        staggerState = GetComponent<Skeleton_StaggerState>();

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

        if (staggerState == null)
        {
            Debug.LogError("Skeleton_StaggerState is missing from the skeleton");
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
            case EnemyState.Patrol:
                patrolState.UpdateState();
                break;

            case EnemyState.Chase:
                chaseState.UpdateState();
                break;

            case EnemyState.Attack:
                attackState.UpdateState();
                break;
            case EnemyState.Stagger:
                staggerState.UpdateState();
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

    public void Stagger()
    {
        if (isDead)
            return;

        ChangeState(EnemyState.Stagger);
    }
}
