using UnityEngine;
using UnityEngine.AI;

public class Goblin_ChaseState : MonoBehaviour
{
    private Goblin_Behaviour Goblin;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private Collider GoblinCollider;
    private Collider playerCollider;

    public float chaseSpeed = 3.5f;
    public float attackDistance = 0.8f;

    void Start()
    {
        Goblin = GetComponent<Goblin_Behaviour>();

        agent = GetComponent<NavMeshAgent>();

        animator = GetComponentInChildren<Animator>();

        GoblinCollider = GetComponentInChildren<Collider>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerCollider = player.GetComponentInChildren<Collider>();
        }
        else
        {
            Debug.LogError("Player with tag 'Player' was not found");
        }
    }

    public void UpdateState()
    {
        if (player == null)
        {
            StopChasing();
            Goblin.ChangeState(Goblin_Behaviour.GoblinState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > Goblin.chaseRadius) //player has left the chase radius
        {
            StopChasing();
            Goblin.ChangeState(Goblin_Behaviour.GoblinState.Patrol);
            return;
        }

        if (GetColliderDistance() <= attackDistance) //player is close enough to attack
        {
            StopChasing();
            Goblin.ChangeState(Goblin_Behaviour.GoblinState.Attack);
            return;
        }

        MoveTowardsPlayer();  //chase the player using the NavMesh
    }

    void MoveTowardsPlayer()
    {
        if (agent == null || !agent.isOnNavMesh)
        {
            return;
        }

        agent.isStopped = false;
        agent.speed = chaseSpeed;

        //stop before reaching the player
        agent.stoppingDistance = attackDistance + 0.2f;
        agent.SetDestination(player.position);

        //play walking animation
        if (animator != null)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("runDaggers"))
            {
                animator.Play("runDaggers");
            }
        }
    }

    void StopChasing()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }

        if (animator != null)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("idleProtectedDaggers"))
            {
                animator.Play("idleProtectedDaggers");
            }
        }
    }

    float GetColliderDistance()
    {
        if (GoblinCollider == null || playerCollider == null)
        {
            return Vector3.Distance(transform.position, player.position);
        }

        Vector3 GoblinPoint = GoblinCollider.ClosestPoint(playerCollider.transform.position);
        Vector3 playerPoint = playerCollider.ClosestPoint(GoblinPoint);
        return Vector3.Distance(GoblinPoint, playerPoint);
    }
}
