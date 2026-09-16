using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 2;
    public int health;
    public float respawnTime = 10f;
    private Skeleton_Behaviour skeleton;
    private bool isDead = false;
    private Animator animator;

    void Start()
    {
        skeleton = GetComponent<Skeleton_Behaviour>();
        health = maxHealth;

        animator = GetComponent<Animator>();

    }

    public void TakeDamage(int amount, Vector3 attackerPosition)
    {
        if (isDead) {  return; }

        health -= amount;

        Debug.Log("Skeleton Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (skeleton != null)
        {
            skeleton.isDead = true;
        }

        Debug.Log("Skeleton died");

        if (animator != null)
        {
            animator.Play("1HandedDeath", 0, 0f);
        }

        Invoke(nameof(Respawn), respawnTime);
    }

    void Respawn()
    {
        health = maxHealth;
        isDead = false;

        if (skeleton != null)
        {
            skeleton.isDead = false;
            skeleton.currentState = Skeleton_Behaviour.EnemyState.Patrol;
        }

        Debug.Log("Skeleton respawning. Health: " + health);

        if (animator != null)
        {
            animator.Play("ComeOutOfTheGround1Handed_A");
        }
    }
}
