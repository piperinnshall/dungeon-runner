using UnityEngine;

public class Goblin_Behaviour : MonoBehaviour
{
    public enum GoblinState
    {
        Patrol,
        Chase,
        Attack
    }
    public bool isDead = false;
    public GoblinState currentState = GoblinState.Patrol;
    public float chaseRadius = 15f;
    private Goblin_PatrolState patrolState;
    private Goblin_ChaseState chaseState;
    private Goblin_AttackState attackState;

    void Start()
    {
        patrolState = GetComponent<Goblin_PatrolState>();
        chaseState = GetComponent<Goblin_ChaseState>();
        attackState = GetComponent<Goblin_AttackState>();

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
        }
    }

    public void ChangeState(GoblinState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        Debug.Log(gameObject.name + " changed state to " + currentState);
    }
}
