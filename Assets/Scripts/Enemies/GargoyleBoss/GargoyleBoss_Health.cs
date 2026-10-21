using UnityEngine;

public class GargoyleBossHealth : MonoBehaviour, EnemyHealth
{
    public int maxHealth = 8;
    public int health;
    private GargoyleBoss_Behaviour GargoyleBoss;
    private bool isDead = false;
    // private Animator animator;

    void Start()
    {
        GargoyleBoss = GetComponent<GargoyleBoss_Behaviour>();
        health = maxHealth;

        // animator = GetComponent<Animator>();

    }

    public void TakeDamage(int amount, Vector3 attackerPosition, GameObject attacker)
    {
        if (isDead) {  return; }

        health -= amount;

        Debug.Log("GargoyleBoss Health: " + health);

        if (health <= 0)
        {
            GargoyleBossDie();
        }
        // animator.Play("getHitDaggers");
    }

    void GargoyleBossDie()
    {
        isDead = true;

        if (GargoyleBoss != null)
        {
            GargoyleBoss.isDead = true;
        }

        Debug.Log("GargoyleBoss died");
        /*
        if (animator != null)
        {
            animator.Play("deathDaggers");
            Destroy(gameObject, 2f);
        }
        */

    }
}

