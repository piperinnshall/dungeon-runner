using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;
    public int health;
    public Boolean isDead = false;
    private CombatState playerCombat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TakeDamage(int amount, Vector3 attackerPosition)
    {
        if (isDead)
            return;

        // If blocking...
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Check if attacker is infront of player
            Vector3 dirToAttacker = (attackerPosition - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, dirToAttacker);

            if (dotProduct > 0.5f) // > 0.5 means within roughly a 120-degree front cone
            {
                Debug.Log("Attack blocked completely!");
                // Play a shield impact particle or sound effect here
                return;
            }
        }

        // Apply Damage
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
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
