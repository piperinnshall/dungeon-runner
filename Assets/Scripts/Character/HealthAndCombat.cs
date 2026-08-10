using UnityEngine;

public class HealthAndCombat : MonoBehaviour
{
    public int health = 5;
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    public int attackDamage = 1;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (health < 0)
        //{
        //    return ;
        //}

        if(Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
                nextAttackTime = Time.time + 1f/attackRate;
            }
        }
    }


    void Attack()
    {
        // Play attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Detect all enemies within the sphere radius around AttackPoint
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Apply damage to each detected enemy
        foreach (Collider enemy in hitEnemies)
        {
            //EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            //if (enemyHealth != null)
            //{
            //    enemyHealth.TakeDamage(attackDamage);
            //}
        }
    }
}
