using UnityEngine;
using UnityEngine.AI;

public class Gargoyle_ChaseState : MonoBehaviour
{
    private Gargoyle_Behaviour Gargoyle;

    private Transform player;
    private NavMeshAgent agent;
    // private Animator animator;

    private Collider GargoyleCollider;
    private Collider playerCollider;

    public float chaseSpeed = 1.5f;
    public float attackDistance = 0.8f;

    void Start()
    {
        Gargoyle = GetComponent<Gargoyle_Behaviour>();

        agent = GetComponent<NavMeshAgent>();

        // animator = GetComponentInChildren<Animator>();

        GargoyleCollider = GetComponentInChildren<Collider>();

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
            Gargoyle.ChangeState(Gargoyle_Behaviour.GargoyleState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > Gargoyle.chaseRadius) //player has left the chase radius
        {
            StopChasing();
            Gargoyle.ChangeState(Gargoyle_Behaviour.GargoyleState.Patrol);
            return;
        }

        if (GetColliderDistance() <= attackDistance) //player is close enough to attack
        {
            StopChasing();
            Gargoyle.ChangeState(Gargoyle_Behaviour.GargoyleState.Attack);
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
        /*
        if (animator != null)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("runDaggers"))
            {
                animator.Play("runDaggers");
            }
        }
        */
    }

    void StopChasing()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }
        /*
        if (animator != null)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("idleProtectedDaggers"))
            {
                animator.Play("idleProtectedDaggers");
            }
        }
        */
    }

    float GetColliderDistance()
    {
        if (GargoyleCollider == null || playerCollider == null)
        {
            return Vector3.Distance(transform.position, player.position);
        }

        Vector3 GargoylePoint = GargoyleCollider.ClosestPoint(playerCollider.transform.position);
        Vector3 playerPoint = playerCollider.ClosestPoint(GargoylePoint);
        return Vector3.Distance(GargoylePoint, playerPoint);
    }
}
