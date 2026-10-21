using System;
using UnityEditor;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;
    public int health;
    public Boolean isDead = false;
    private Combat combatScript;

    [SerializeField] Movement moveScript;

    public Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        _animator = GetComponent<Animator>();
        combatScript = GetComponent<Combat>();
    }

    public void TakeDamage(int amount, Vector3 attackerPosition, GameObject attacker)
    {
        if (isDead)
            return;

        // If blocking...
        if (moveScript.isBlocking) //don't need to check if true because it won't this returns bool
        {
            // Check if attacker is infront of player
            Vector3 dirToAttacker = (attackerPosition - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, dirToAttacker);

            if (dotProduct > 0.5f) // > 0.5 means within roughly a 120-degree front cone
            {
                Debug.Log("Attack blocked completely!");

                EnemyStagger enemy = attacker.GetComponentInParent<EnemyStagger>();

                if (enemy != null && combatScript != null && Time.time - combatScript.blockStartTime <= 1.5f)
                {
                    enemy.Stagger();
                    Debug.Log("Perfect block! Enemy staggered!");
                }

                return;
            }
        }

        moveScript.canMove = false;
        // Apply Damage
        health -= amount;
        Invoke(nameof(CanMoveAgain), 0.05f);
        _animator.SetTrigger("HitTrigger");
        if (health <= 0)
        {
            Die();
        }
    }

    void CanMoveAgain()
    {
        moveScript.canMove = true;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player is dead");

        // Play death anaimation
        // Disable movement
        // Go to death menu/reset level
        gameObject.SetActive(false);
    }
}
