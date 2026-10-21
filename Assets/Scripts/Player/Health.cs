using System;
// (Removed unused "using UnityEditor;": it's editor-only and makes a game BUILD fail.)
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;
    public int health;
    public Boolean isDead = false;

    [SerializeField] Movement moveScript;

    public Animator _animator;

    // Optional combat sounds (PlayerSounds component on the player)
    PlayerSounds playerSounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        _animator = GetComponent<Animator>();
        playerSounds = GetComponent<PlayerSounds>();
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
        if (moveScript.isBlocking == true)
        {
            // Check if attacker is infront of player
            Vector3 dirToAttacker = (attackerPosition - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, dirToAttacker);

            if (dotProduct > 0.5f) // > 0.5 means within roughly a 120-degree front cone
            {
                Debug.Log("Attack blocked completely!");
                // Play a shield impact particle or sound effect here
                if (playerSounds != null) playerSounds.PlayBlock();
                return;
            }
        }

        moveScript.canMove = false;
        // Apply Damage
        health -= amount;
        if (playerSounds != null) playerSounds.PlayHurt(); // random hurt shout
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
