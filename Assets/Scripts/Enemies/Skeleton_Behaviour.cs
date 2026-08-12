using UnityEngine;

public class Skeleton_Behaviour : MonoBehaviour
{
    private Animator animator;
    private Transform player;
    private CharacterController controller;
    private Collider skeletonCollider;
    private Collider playerCollider;

    public float speed = 2f;
    public float rayDistance = 10f; // change if we need further view distance idk how far skeletons can see
    public float viewAngle = 140f;
    public float attackDistance = 0.5f;

    private bool isWalking = false;
    private bool isAttacking = false;

    private float attackStartTime;
    private float attackLength;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        skeletonCollider = GetComponentInChildren<Collider>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerCollider =
                player.GetComponentInChildren<Collider>();
        }
        else
        {
            Debug.LogError("Player with tag 'Player' was NOT found!");
        }
    }

    void Update()
    {
        if (player == null || playerCollider == null)
        {
            return;
        }

        // while attacking stay still and wait for animation to finish
        if (isAttacking)
        {
            CheckAttackFinished();
            return;
        }

        Vector3 direction =
            (player.position - transform.position).normalized;

        //check 140 degree field of view
        float angle = Vector3.Angle(
            transform.forward,
            direction
        );

        if (angle > viewAngle / 2f)
        {
            StopWalking();
            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(
            transform.position,
            direction,
            rayDistance
        );

        RaycastHit? playerHit = null;
        float closestDistance = rayDistance;

        foreach (RaycastHit hit in hits)
        {
            // ignore enemies
            if (hit.collider.CompareTag("Enemy"))
            {
                continue;
            }

            // ignore this skeleton. Will change when we have multiple skeletons
            if (hit.collider.transform.root == transform.root)
            {
                continue;
            }

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                playerHit = hit;
            }
        }

        if (playerHit.HasValue)
        {
            RaycastHit hit = playerHit.Value;

            Debug.DrawRay(
                transform.position,
                direction * hit.distance,
                Color.red
            );

            if (hit.collider.transform.root.CompareTag("Player"))
            {
                float distance = GetColliderDistance();

                if (distance <= attackDistance)
                {
                    AttackPlayer();
                }
                else
                {
                    WalkTowardsPlayer();
                }
            }
            else
            {
                StopWalking();
            }
        }
        else
        {
            Debug.DrawRay(
                transform.position,
                direction * rayDistance,
                Color.yellow
            );

            StopWalking();
        }
    }

    void WalkTowardsPlayer()
    {
        if (isAttacking)
        {
            return;
        }

        float distance = GetColliderDistance();

        if (distance <= attackDistance)
        {
            AttackPlayer();
            return;
        }

        Vector3 lookDirection =
            player.position - transform.position;

        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            transform.rotation =
                Quaternion.LookRotation(lookDirection);
        }

        float moveDistance =
            speed * Time.deltaTime;

        float allowedDistance =
            distance - attackDistance;

        if (moveDistance > allowedDistance)
        {
            moveDistance = allowedDistance;
        }

        if (moveDistance > 0f)
        {
            Vector3 movement =
                transform.forward * moveDistance;

            if (controller != null)
            {
                controller.Move(movement);
            }
            else
            {
                transform.position += movement;
            }
        }

        if (!isWalking)
        {
            animator.Play("1HandedWalk");
            isWalking = true;
        }
    }

    void AttackPlayer()
    {
        if (isAttacking)
        {
            return;
        }

        isWalking = false;
        isAttacking = true;

        animator.Play("1HandedAttack1", 0, 0f);

        
        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        attackLength = state.length;
        attackStartTime = Time.time;
    }

    void CheckAttackFinished()
    {
        //wait til animation is done
        if (Time.time - attackStartTime < attackLength)
        {
            return;
        }

        // attack finished
        isAttacking = false;

        //checks if player is still infront
        Vector3 direction =
            (player.position - transform.position).normalized;

        float angle = Vector3.Angle(
            transform.forward,
            direction
        );

        if (angle <= viewAngle / 2f)
        {
            float distance = GetColliderDistance();

            // Player is still close enough.
            if (distance <= attackDistance)
            {
                AttackPlayer();
                return;
            }
        }

        // player out of view
        StopWalking();
    }

    float GetColliderDistance()
    {
        Vector3 skeletonPoint =
            skeletonCollider.ClosestPoint(
                playerCollider.transform.position
            );

        Vector3 playerPoint =
            playerCollider.ClosestPoint(
                skeletonPoint
            );

        return Vector3.Distance(
            skeletonPoint,
            playerPoint
        );
    }

    void StopWalking()
    {
        if (!isAttacking)
        {
            isWalking = false;
            animator.Play("Idle1Handed");
        }
    }
}
