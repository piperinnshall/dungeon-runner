using UnityEngine;

public class BoomerangProjectile : MonoBehaviour
{
    // The distance the boomerang will travel before returning
    float travelDistance = 10f;
    float speed = 10f;
    bool isReturning = false;
    int damage = 1;
    Vector3 startPosition;
    Vector3 travelDirection;

    Transform player;
    BoxCollider boxCollider;
    Equipment equipment;
    Movement movementScript;

    public void Initialize(Vector3 direction)
    {
        travelDirection = direction.normalized;
        transform.Rotate(0, 0, 90);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        equipment = player.GetComponentInParent<Equipment>();
        movementScript = player.GetComponentInParent<Movement>();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isReturning)
        {
            // Move towards the player
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else
        {
            // Move forward
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPosition) >= travelDistance)
            {
                isReturning = true;
            }
        }
        OnCollide();
    }

    void OnCollide()
    {
        // Check for collisions with enemies, walls, and the player, when a collision occurs, the boomerang returns to the player
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var hitCollider in hitColliders)
        {
            // If the collider is an enemy, damage them
            EnemyHealth enemyHealth = hitCollider.GetComponentInParent<EnemyHealth>();
            if (hitCollider.CompareTag("Enemy") && enemyHealth != null)
            {
                Debug.Log(enemyHealth);
                enemyHealth.TakeDamage(damage, transform.position, gameObject);
                isReturning = true;
            }
            // if the collider is a player and the boomerang is returning, destroy the boomerang projectile and equip the boomerang back to the player
            else if (hitCollider.CompareTag("Player") && isReturning)
            {
                equipment.EquipBoomerang();
                movementScript.thrownBoomerang = false;
                Destroy(gameObject);
            }
            // if the collider is a wall, bounce off the wall and return to the player
            else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                isReturning = true;
            }
            //Debug.Log("Collided with: " + hitCollider.name);
        }
    }
}
