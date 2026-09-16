using UnityEngine;

public class Skeleton_AttackState : MonoBehaviour
{
    private Skeleton_Behaviour skeleton;

    private Transform player;
    private Combat playerCombat;
    private Collider skeletonCollider;
    private Collider playerCollider;
    private Health playerHealth;
    private Animator animator;

    public float attackDistance = 1.5f;
    public float attackCooldown = 1.2f; //animation duration is 0.66 seconds trust

    private bool attacking = false;
    private float attackTimer = 0f;

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
            playerCombat = playerObject.GetComponent<Combat>();
            playerHealth = playerObject.GetComponent<Health>();
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

        // Player left the chase radius
        if (distanceToPlayer > skeleton.chaseRadius)
        {
            attacking = false;
            attackTimer = 0f;
            skeleton.ChangeState(Skeleton_Behaviour.EnemyState.Patrol);
            return;
        }

        // Player moved out of attack distance
        if (GetColliderDistance() > attackDistance)
        {
            attacking = false;
            attackTimer = 0f;
            skeleton.ChangeState(Skeleton_Behaviour.EnemyState.Chase);
            return;
        }

        // Always face the player while in attack range
        FacePlayer();

        // Count down the attack timer
        if (attacking)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                attacking = false;
            }

            return;
        }

        Attack();
    }

    void Attack()
    {
        attacking = true;
        attackTimer = attackCooldown;

        if (animator != null)
        {
            animator.Play("1HandedAttack1", 0, 0f);
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(1, transform.position);
            Debug.Log("Player Health: " + playerHealth.health);
        }
    }
    public void DealDamage()
    {
        if (playerCombat != null)
        {
            playerCombat.TakeDamage(1);
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
}
