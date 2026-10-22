using UnityEngine;

public class Goblin_Behaviour : MonoBehaviour, EnemyStagger
{
    public enum GoblinState
    {
        Patrol,
        Chase,
        Attack,
        Stagger
    }

    public bool isDead = false;
    public GoblinState currentState = GoblinState.Patrol;
    public float chaseRadius = 15f;
    public float staggerDuration = 1f;


    private Goblin_PatrolState patrolState;
    private Goblin_ChaseState chaseState;
    private Goblin_AttackState attackState;
    private Goblin_StaggerState staggerState;

    void Start()
    {
        patrolState = GetComponent<Goblin_PatrolState>();
        chaseState = GetComponent<Goblin_ChaseState>();
        attackState = GetComponent<Goblin_AttackState>();
        staggerState = GetComponent<Goblin_StaggerState>();

        if (patrolState == null)
        {
            Debug.LogError("Goblin_PatrolState is missing from the Goblin");
        }

        if (chaseState == null)
        {
            Debug.LogError("Goblin_ChaseState is missing from the Goblin");
        }

        if (attackState == null)
        {
            Debug.LogError("Goblin_AttackState is missing from the Goblin");
        }

        if (staggerState == null)
        {
            Debug.LogError("Goblin_StaggerState is missing from the Goblin");
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
            case GoblinState.Patrol:
                patrolState.UpdateState();
                break;

            case GoblinState.Chase:
                chaseState.UpdateState();
                break;

            case GoblinState.Attack:
                attackState.UpdateState();
                break;
            case GoblinState.Stagger:
                staggerState.UpdateState();
                break;
        }
    }

    public void ChangeState(GoblinState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
        if (newState == GoblinState.Stagger && staggerState != null)
        {
            staggerState.EnterState();
        }
        Debug.Log(gameObject.name + " changed state to " + currentState);
    }

    public void Stagger()
    {
        if (isDead)
            return;

        ChangeState(GoblinState.Stagger);
    }
}
