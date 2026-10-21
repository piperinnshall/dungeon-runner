
using UnityEngine;

public class GargoyleBoss_AttackState : MonoBehaviour
{
    private GargoyleBoss_Behaviour GargoyleBoss;
    private Transform player;
    private Collider GargoyleBossCollider;
    private Collider playerCollider;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    public float projectileSpeed = 12f;
    public float attackRange = 10f;
    public float attackCooldown = 3.2f;

    private bool attacking = false;
    private float attackTimer = 0f;

    void Start()
    {
        GargoyleBoss = GetComponent<GargoyleBoss_Behaviour>();
        GargoyleBossCollider = GetComponentInChildren<Collider>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerCollider = playerObject.GetComponentInChildren<Collider>();
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
            GargoyleBoss.ChangeState(GargoyleBoss_Behaviour.GargoyleBossState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > GargoyleBoss.chaseRadius)
        {
            attacking = false;
            attackTimer = 0f;
            GargoyleBoss.ChangeState(GargoyleBoss_Behaviour.GargoyleBossState.Patrol);
            return;
        }

        if (distanceToPlayer > attackRange)
        {
            attacking = false;
            attackTimer = 0f;
            GargoyleBoss.ChangeState(GargoyleBoss_Behaviour.GargoyleBossState.Chase);
            return;
        }

        FacePlayer();

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

        if (projectilePrefab == null || firePoint == null || player == null)
        {
            Debug.LogWarning("Assign Projectile Prefab and Fire Point on GargoyleBoss_AttackState.");
            return;
        }

        Vector3 direction = (player.position - firePoint.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position,Quaternion.LookRotation(direction));

        GargoyleBossProjectile projectileScript = projectile.GetComponent<GargoyleBossProjectile>();

        if (projectileScript != null)
        {
            projectileScript.speed = projectileSpeed;
            projectileScript.Launch(direction);
        }
        else
        {
            Debug.LogError("Projectile prefab is missing GargoyleBossProjectile.");
            Destroy(projectile);
        }
    }

    void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
