using UnityEngine;
using UnityEngine.AI;

public class Skeleton_ChaseState : MonoBehaviour
{
    private Skeleton_Behaviour skeleton;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private Collider skeletonCollider;
    private Collider playerCollider;

    public float chaseSpeed = 3f;
    public float chaseRadius = 10f;
    public float attackDistance = 1.5f;

    void Start()
    {
        skeleton = GetComponent<Skeleton_Behaviour>();

        agent = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();

        skeletonCollider = GetComponentInChildren<Collider>();

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
            skeleton.ChangeState(Skeleton_Behaviour.EnemyState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        //player has left the chase radius
        if (distanceToPlayer > chaseRadius)
        {
            StopChasing();
            skeleton.ChangeState(Skeleton_Behaviour.EnemyState.Patrol);
            return;
        }

        //player is close enough to attack
        if (GetColliderDistance() <= attackDistance)
        {
            StopChasing();
            skeleton.ChangeState(Skeleton_Behaviour.EnemyState.Attack);
            return;
        }

        //chase the player using the NavMesh
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

        agent.SetDestination(player.position);

        //play walking animation
        if (animator != null)
        {
            animator.Play("1HandedWalk");
        }
    }

    void StopChasing()
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

    float GetColliderDistance()
    {
        if (skeletonCollider == null || playerCollider == null)
        {
            return Vector3.Distance(transform.position, player.position);
        }

        Vector3 skeletonPoint = skeletonCollider.ClosestPoint(playerCollider.transform.position);

        Vector3 playerPoint = playerCollider.ClosestPoint(skeletonPoint);

        return Vector3.Distance(skeletonPoint, playerPoint);
    }
}
