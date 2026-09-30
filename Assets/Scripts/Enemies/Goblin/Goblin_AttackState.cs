using UnityEngine;

public class Goblin_AttackState : MonoBehaviour
{
    private Goblin_Behaviour Goblin;

    private Transform player;
    private Combat playerCombat;
    private Collider GoblinCollider;
    private Collider playerCollider;
    private Health playerHealth;
    private Animator animator;

    public float attackDistance = 1.5f;
    public float attackCooldown = 3.2f; //animation duration is 0.66 seconds trust

    private bool attacking = false;
    private float attackTimer = 0f;

    void Start()
    {
        Goblin = GetComponent<Goblin_Behaviour>();

        animator = GetComponent<Animator>();

        GoblinCollider = GetComponentInChildren<Collider>();

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
            Goblin.ChangeState(Goblin_Behaviour.GoblinState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Player left the chase radius
        if (distanceToPlayer > Goblin.chaseRadius)
        {
            attacking = false;
            attackTimer = 0f;
            Goblin.ChangeState(Goblin_Behaviour.GoblinState.Patrol);
            return;
        }

        // Player moved out of attack distance
        if (GetColliderDistance() > attackDistance)
        {
            attacking = false;
            attackTimer = 0f;
            Goblin.ChangeState(Goblin_Behaviour.GoblinState.Chase);
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
            animator.Play("attack2ForwardDaggers", 0, 0f);
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
        if (GoblinCollider == null || playerCollider == null)
        {
            return Vector3.Distance(transform.position, player.position);
        }

        Vector3 GoblinPoint = GoblinCollider.ClosestPoint(playerCollider.transform.position);

        Vector3 playerPoint = playerCollider.ClosestPoint(GoblinPoint);

        return Vector3.Distance(GoblinPoint, playerPoint);
    }
}
