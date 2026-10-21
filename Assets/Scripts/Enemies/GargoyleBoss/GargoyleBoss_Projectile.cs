
using UnityEngine;

public class GargoyleBossProjectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Health playerHealth = collision.collider.GetComponentInParent<Health>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage, transform.position, gameObject);
        }

        Destroy(gameObject);
    }
}
