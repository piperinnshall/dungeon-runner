using UnityEngine;
using UnityEngine.AI;

public class Goblin_PatrolState : MonoBehaviour
{
    private Goblin_Behaviour Goblin;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    public float patrolSpeed = 2f;
    public float detectionRadius = 5f;

    public Transform patrolPointA;
    public Transform patrolPointB;

    private bool goingToPointA = true;

    void Start()
    {
        Goblin = GetComponent<Goblin_Behaviour>();

        agent = GetComponent<NavMeshAgent>();

        animator = GetComponentInChildren<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        if (agent != null)
        {
            agent.speed = patrolSpeed;
        }
    }

    public void UpdateState()
    {

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Player is inside detection radius
            if (distanceToPlayer <= detectionRadius)
            {

                    Goblin.ChangeState(Goblin_Behaviour.GoblinState.Chase);
                    return;
                
            }
        }

        if (patrolPointA == null || patrolPointB == null)
        {
            StopPatrol();
            return;
        }

        Patrol();
    }

    void Patrol()
    {
        Transform targetPoint;

        if (goingToPointA)
        {
            targetPoint = patrolPointA;
        }
        else
        {
            targetPoint = patrolPointB;
        }

        if (agent == null || !agent.isOnNavMesh)
        {
            return;
        }

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0f;

        agent.SetDestination(targetPoint.position);

        // Play walking animation
        if (animator != null)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("walkForwardDaggers"))
            {
                animator.Play("walkForwardDaggers");
            }
        }

        if (agent.remainingDistance <= 0.5f && !agent.pathPending)
        {
            goingToPointA = !goingToPointA;
            agent.ResetPath();
        }
    }

    void StopPatrol()
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

    bool HasLineOfSight()
    {
        Vector3 direction = player.position - transform.position;

        float distance = direction.magnitude;

        direction.Normalize();

        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {
            // The first thing hit is the player
            if (hit.collider.transform.root.CompareTag("Player"))
            {
                return true;
            }

            // Something else is blocking the player
            return false;
        }

        return false;
    }
}
