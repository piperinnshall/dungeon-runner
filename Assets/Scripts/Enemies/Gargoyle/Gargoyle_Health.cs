using UnityEngine;

public class GargoyleHealth : MonoBehaviour, EnemyHealth
{
    public int maxHealth = 1;
    public int health;
    private Gargoyle_Behaviour Gargoyle;
    private bool isDead = false;
    // private Animator animator;

    void Start()
    {
        Gargoyle = GetComponent<Gargoyle_Behaviour>();
        health = maxHealth;

        // animator = GetComponent<Animator>();

    }

    public void TakeDamage(int amount, Vector3 attackerPosition, GameObject attacker)
    {
        if (isDead) {  return; }

        health -= amount;

        Debug.Log("Gargoyle Health: " + health);

        if (health <= 0)
        {
            GargoyleDie();
        }
        // animator.Play("getHitDaggers");
    }

    void GargoyleDie()
    {
        isDead = true;

        if (Gargoyle != null)
        {
            Gargoyle.isDead = true;
        }

        Debug.Log("Gargoyle died");
        /*
        if (animator != null)
        {
            animator.Play("deathDaggers");
            Destroy(gameObject, 2f);
        }
        */

    }
}

