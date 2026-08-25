using UnityEngine;

public class Skeleton_Behaviour : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    private EnemyState currentState = EnemyState.Patrol;

    private Transform player;
    private CharacterController controller;

    private Collider skeletonCollider;
    private Collider playerCollider;

    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;

    public float detectionRadius = 5f;
    public float chaseRadius = 10f;
    public float attackDistance = 1.5f;

    public Transform patrolPointA;
    public Transform patrolPointB;

    private bool goingToPointA = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        skeletonCollider = GetComponentInChildren<Collider>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerCollider = player.GetComponentInChildren<Collider>();
        }
        else
        {
            Debug.LogError("Player with tag 'Player' was not found"); //assign the player tag to the player object if this shows up
        }

        if (patrolPointA == null)
        {
            Debug.LogError("Patrol Point A has not been assigned"); //assign these on in the inspector if this shows up
        }

        if (patrolPointB == null)
        {
            Debug.LogError("Patrol Point B has not been assigned");
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                Chase();
                break;

            case EnemyState.Attack:
                Attack();
                break;
        }
    }

    void Patrol()
    {
        if (patrolPointA == null || patrolPointB == null)
        {
            return;
        }

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(
                transform.position,
                player.position
            );

            //player is inside detection radius
            if (distanceToPlayer <= detectionRadius)
            {
                //only check line of sight while patrolling
                if (HasLineOfSight())
                {
                    ChangeState(EnemyState.Chase);
                    return;
                }
            }
        }

        Transform targetPoint;

        if (goingToPointA)
        {
            targetPoint = patrolPointA;
        }
        else
        {
            targetPoint = patrolPointB;
        }

        Vector3 direction = targetPoint.position - transform.position;

        direction.y = 0f;

        if (direction.magnitude <= 0.5f)
        {
            goingToPointA = !goingToPointA;
            return;
        }

        direction.Normalize();

        transform.rotation = Quaternion.LookRotation(direction);

        Vector3 movement = direction * patrolSpeed * Time.deltaTime;

        if (controller != null)
        {
            controller.Move(movement);
        }
        else
        {
            transform.position += movement;
        }
    }

    void Chase()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //player has left the chase radius
        if (distanceToPlayer > chaseRadius)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }

        //player is close enough to attack
        if (GetColliderDistance() <= attackDistance)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        MoveTowards(player.position, chaseSpeed);
    }

    void Attack()
    {
        if (player == null)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //player left chase radius
        if (distanceToPlayer > chaseRadius)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }

        //player moved out of attack distance
        if (GetColliderDistance() > attackDistance)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        FacePlayer();
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

    void MoveTowards(Vector3 targetPosition, float movementSpeed)
    {
        Vector3 direction = targetPosition - transform.position;

        direction.y = 0f;

        if (direction.magnitude <= 0.01f)
        {
            return;
        }

        direction.Normalize();

        transform.rotation = Quaternion.LookRotation(direction);

        Vector3 movement = direction * movementSpeed * Time.deltaTime;

        if (controller != null)
        {
            controller.Move(movement);
        }
        else
        {
            transform.position += movement;
        }
    }

    void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;

        direction.y = 0f;

        if (direction.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
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

    void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        Debug.Log(gameObject.name + " changed state to " + currentState);
    }

    void OnDrawGizmosSelected()
    {
        // Detection radius
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Chase radius
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
