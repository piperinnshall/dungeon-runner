using UnityEngine;

public class Skeleton_AttackState : MonoBehaviour
{
    private Skeleton_Behaviour skeleton;

    private Transform player;

    private Collider skeletonCollider;
    private Collider playerCollider;

    private Animator animator;

    public float attackDistance = 1.5f;

    public float attackCooldown = 1f;

    private bool attacking = false;

    void Start()
    {
        skeleton = GetComponent<Skeleton_Behaviour>();

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
            skeleton.ChangeState(Skeleton_Behaviour.EnemyState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //player left the chase radius
        if (distanceToPlayer > skeleton.chaseRadius)
        {
            attacking = false;
            skeleton.ChangeState(Skeleton_Behaviour.EnemyState.Patrol);
            return;
        }

        //player moved out of attack distance
        if (GetColliderDistance() > attackDistance)
        {
            attacking = false;
            skeleton.ChangeState(Skeleton_Behaviour.EnemyState.Chase);
            return;
        }

        //always face the player while in attack range
        FacePlayer();

        //attack is still playing
        if (attacking)
        {
            return;
        }

        Attack();
    }

    void Attack()
    {
        attacking = true;

        if (animator != null)
        {
            animator.Play("1HandedAttack1", 0, 0f);
        }

        Invoke("FinishAttack", attackCooldown);
    }

    void FinishAttack()
    {
        attacking = false;

        if (player == null)
        {
            skeleton.ChangeState(Skeleton_Behaviour.EnemyState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //player has moved away
        if (distanceToPlayer > skeleton.chaseRadius)
        {
            skeleton.ChangeState(Skeleton_Behaviour.EnemyState.Patrol);

            return;
        }

        //player is still close enough to attack
        if (GetColliderDistance() <= attackDistance)
        {
            return;
        }

        skeleton.ChangeState(Skeleton_Behaviour.EnemyState.Chase);
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
}
