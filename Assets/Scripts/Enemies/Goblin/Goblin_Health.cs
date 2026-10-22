using System.Collections.Generic;
using UnityEngine;

public class GoblinHealth : MonoBehaviour, EnemyHealth
{
    public int maxHealth = 1;
    public int health;
    private Goblin_Behaviour Goblin;
    private bool isDead = false;
    private Animator animator;

    [Header("Coin drop")]
    [Tooltip("Fewest coins a goblin gives when defeated.")]
    public int minCoins = 1;
    [Tooltip("Most coins a goblin gives when defeated.")]
    public int maxCoins = 2;
    [Tooltip("Optional sound played when the coins are collected.")]
    public AudioClip coinSound;

    void Start()
    {
        Goblin = GetComponent<Goblin_Behaviour>();
        health = maxHealth;
        animator = GetComponent<Animator>();

    }

    public void TakeDamage(int amount, Vector3 attackerPosition, GameObject attacker)
    {
        if (isDead) {  return; }

        health -= amount;

        Debug.Log("Goblin Health: " + health);

        if (health <= 0)
        {
            GoblinDie();
            return; // don't let the hit animation replace the death animation
        }
        animator.Play("getHitDaggers");
    }

    void GoblinDie()
    {
        isDead = true;

        if (Goblin != null)
        {
            Goblin.isDead = true;
        }

        Debug.Log("Goblin died");

        DropCoins();

        if (animator != null)
        {
            animator.Play("deathDaggers");
            Destroy(gameObject, 2f);
        }

    }

    // Gives the player 1 or 2 coins (straight into their coin total, the HUD counter updates).
    void DropCoins()
    {
        int coins = Random.Range(minCoins, maxCoins + 1);
        if (coins <= 0) return;

        TreasureManager.Shared.Open(new List<ICoinPacket> { new ICoinPacket.Cheap(coins) });
        Debug.Log("Goblin dropped " + coins + " coin(s)");

        if (coinSound != null)
            AudioSource.PlayClipAtPoint(coinSound, transform.position);
    }
}

