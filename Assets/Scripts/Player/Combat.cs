using UnityEngine;

public enum CombatState
{
    Idle,
    Attacking,
    Blocking
}

public class Combat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCoolldown = 0.5f;
    public int attackDamage = 1;
    public LayerMask enemyLayers;

    [Header("References")]
    public Transform attackPoint;
    public Animator animator;
    public Transform playerCamera;

    private CombatState currentState = CombatState.Idle;
    private float nextAttackTime = 0f;


    void Update()
    {
        // Blocking
        if (Input.GetMouseButton(1) &&
            currentState != CombatState.Attacking)
        {
            StartBlocking();
        }

        if (Input.GetMouseButtonUp(1) &&
            currentState == CombatState.Blocking)
        {
            StopBlocking();
        }

        // Face camera direction while blocking
        if (currentState == CombatState.Blocking)
        {
            FaceCameraDirection();
        }

        // Attack
        if (Time.time >= nextAttackTime &&
            currentState == CombatState.Idle &&
            Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("Player is attacking");

        currentState = CombatState.Attacking;
        nextAttackTime = Time.time + attackCoolldown;

        if (animator != null)
        {
            // animator.SetTrigger("Attack");
        }

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage, transform.position);

                Debug.Log("Skeleton Health: " + enemyHealth.health);
            }
        }

        Invoke(nameof(ResetState), attackCoolldown);
    }

    void StartBlocking()
    {
        if (currentState == CombatState.Blocking)
        {
            return;
        }

        currentState = CombatState.Blocking;

        // Disable movement here if needed
        // animator.SetBool("isBlocking", true);
    }

    void StopBlocking()
    {
        // animator.SetBool("isBlocking", false);

        // Enable movement here if needed

        ResetState();
    }

    void ResetState()
    {
        currentState = CombatState.Idle;
    }

    public void FaceCameraDirection()
    {
        if (playerCamera == null)
        {
            return;
        }

        Vector3 camForward = playerCamera.forward;
        camForward.y = 0f;

        if (camForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }
    }

    public void TakeDamage(int damage)
    {
        Health playerHealth = GetComponent<Health>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage, transform.position);

            Debug.Log("Player Health: " + playerHealth.health);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
