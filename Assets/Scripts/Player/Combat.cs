using Unity.VisualScripting;
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
    public LayerMask enemyLayers; // Layer(s) that enemies are on, this is for collision

    [Header("References")]
    public Transform attackPoint;
    public Animator animator;
    public Transform playerCamera;

    private CombatState currentState = CombatState.Idle;
    private float nextAttackTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Block when holding right mouse button
        if(Input.GetMouseButton(1) && currentState != CombatState.Attacking)
        {
            StartBlocking();
        } else if(Input.GetMouseButton(1) && currentState == CombatState.Blocking)
        {
            StopBlocking();
        }

        if(Time.time >= nextAttackTime && currentState == CombatState.Idle)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Attack();
            }
        }
    }

    void Attack()
    {
        currentState = CombatState.Attacking;
        nextAttackTime = Time.time + attackCoolldown;

        //animator.SetTrigger("Attack");

        // Enemies in range of attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Damage enemies
        foreach(Collider enemy in hitEnemies)
        {
            //EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            //enemyHealth.takeDamage(attackDamage);
        }
        Invoke(nameof(ResetState), attackCoolldown);
    }

    void StartBlocking()
    {
        currentState = CombatState.Blocking;
        // Disable movement
        //animator.setBool("isBlocking, true);
    }

    void StopBlocking()
    {
        //animator.setBool("isBlocking, false);
        // Enable movement
        ResetState();
    }

    void ResetState()
    {
        currentState = CombatState.Idle;
    }

    public void FaceCameraDirection()
    {
        // Get camera foward vector ignoring vertical tilt (Y-axis)
        Vector3 camForward = playerCamera.forward;
        camForward.y = 0f;

        if(camForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            // Smoothly rotate toward camera facing direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        while (currentState == CombatState.Blocking)
        {
            FaceCameraDirection();
        }
    }
}
