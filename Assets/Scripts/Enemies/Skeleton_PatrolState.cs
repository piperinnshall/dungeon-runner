using UnityEngine;
using UnityEngine.AI;

public class Skeleton_PatrolState : MonoBehaviour
{
    private Skeleton_Behaviour skeleton;

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
        skeleton = GetComponent<Skeleton_Behaviour>();

        agent = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player with tag 'Player' was not found"); //Make sure player is tagged correctly in the scene
        }

        if (patrolPointA == null)
        {
            Debug.LogError("Patrol Point A has not been assigned"); //Make sure to assign patrol points in the inspector
        }

        if (patrolPointB == null)
        {
            Debug.LogError("Patrol Point B has not been assigned");
        }

        if (agent != null)
        {
            agent.speed = patrolSpeed;
        }
    }

    public void UpdateState()
    {
        if (patrolPointA == null || patrolPointB == null)
        {
            return;
        }

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            //player is inside detection radius
            if (distanceToPlayer <= detectionRadius)
            {
                //only detect player if there is line of sight
                if (HasLineOfSight())
                {
                    StopPatrol();
                    skeleton.ChangeState(Skeleton_Behaviour.EnemyState.Chase);
                    return;
                }
            }
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

        agent.SetDestination(targetPoint.position);

        //play walking animation
        if (animator != null)
        {
            animator.Play("1HandedWalk");
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
            agent.ResetPath();
        }

        if (animator != null)
        {
            animator.Play("Idle1Handed");
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
            Debug.DrawRay(transform.position, direction * hit.distance, Color.red);

            //the first thing hit is the player
            if (hit.collider.transform.root.CompareTag("Player"))
            {
                return true;
            }

            //something else is blocking the player
            return false;
        }

        return false;
    }
}
