using UnityEngine;

public class BoomerangProjectile : MonoBehaviour
{
    // The distance the boomerang will travel before returning
    float travelDistance = 10f;
    float speed = 10f;
    bool isReturning = false;
    Vector3 startPosition;

    Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isReturning)
        {
            // Move towards the player
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, player.position) < 0.1f)
            {
                Destroy(gameObject);
            }
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

    }
}
