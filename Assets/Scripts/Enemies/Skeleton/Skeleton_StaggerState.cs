using UnityEngine;

public class Skeleton_StaggerState : MonoBehaviour
{
    private Skeleton_Behaviour skeleton;
    private Animator animator;

    public float staggerDuration = 1f;

    private bool staggering = false;
    private float staggerTimer = 0f;

    void Start()
    {
        skeleton = GetComponent<Skeleton_Behaviour>();
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
            animator.Play("1HandedgetHit");
        }

        Debug.Log("Skeleton staggered!");
    }

    public void UpdateState()
    {
        if (!staggering)
            return;

        if (skeleton == null || skeleton.isDead)
            return;

        staggerTimer += Time.deltaTime;

        if (staggerTimer >= staggerDuration)
        {
            staggering = false;
            skeleton.currentState = Skeleton_Behaviour.EnemyState.Chase;
        }
    }

}
