using UnityEngine;

public class GoblinHealth : MonoBehaviour, EnemyHealth
{
    public int maxHealth = 1;
    public int health;
    private Goblin_Behaviour Goblin;
    private bool isDead = false;
    private Animator animator;

    void Start()
    {
        Goblin = GetComponent<Goblin_Behaviour>();
        health = maxHealth;

        animator = GetComponent<Animator>();

    }

    public void TakeDamage(int amount, Vector3 attackerPosition)
    {
        if (isDead) {  return; }

        health -= amount;

        Debug.Log("Goblin Health: " + health);

        if (health <= 0)
        {
            GoblinDie();
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

        if (animator != null)
        {
            animator.Play("deathDaggers");
            Destroy(gameObject, 2f);
        }

    }
}
