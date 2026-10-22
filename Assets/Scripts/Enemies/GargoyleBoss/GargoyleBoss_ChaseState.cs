
using UnityEngine;
using UnityEngine.AI;

public class GargoyleBoss_ChaseState : MonoBehaviour
{
    private GargoyleBoss_Behaviour GargoyleBoss;
    private Transform player;
    private NavMeshAgent agent;

    public float chaseSpeed = 1.5f;
    public float attackDistance = 10f;

    void Start()
    {
        GargoyleBoss = GetComponent<GargoyleBoss_Behaviour>();
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
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
            GargoyleBoss.ChangeState(GargoyleBoss_Behaviour.GargoyleBossState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > GargoyleBoss.chaseRadius)
        {
            StopChasing();
            GargoyleBoss.ChangeState(GargoyleBoss_Behaviour.GargoyleBossState.Patrol);
            return;
        }

        if (distanceToPlayer <= attackDistance)
        {
            StopChasing();
            GargoyleBoss.ChangeState(GargoyleBoss_Behaviour.GargoyleBossState.Attack);
            return;
        }

        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        if (agent == null || !agent.isOnNavMesh)
        {
            return;
        }

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = attackDistance;
        agent.SetDestination(player.position);
    }

    void StopChasing()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }
    }
}
