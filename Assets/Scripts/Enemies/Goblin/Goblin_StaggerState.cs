using UnityEngine;

public class Goblin_StaggerState : MonoBehaviour
{
    private Goblin_Behaviour Goblin;
    private Animator animator;

    public float staggerDuration = 1f;

    private bool staggering = false;
    private float staggerTimer = 0f;

    void Start()
    {
        Goblin = GetComponent<Goblin_Behaviour>();
        animator = GetComponent<Animator>();
    }

    public void EnterState()
    {
        if (staggering)
            return;

        staggering = true;
        staggerTimer = 0f;

        if (animator != null)
        {
            animator.Play("getHitDaggers");
        }

        Debug.Log("Goblin staggered!");
    }

    public void UpdateState()
    {
        if (!staggering)
            return;

        if (Goblin == null || Goblin.isDead)
            return;

        staggerTimer += Time.deltaTime;

        if (staggerTimer >= staggerDuration)
        {
            staggering = false;
            Goblin.currentState = Goblin_Behaviour.GoblinState.Chase;
        }
    }

}
