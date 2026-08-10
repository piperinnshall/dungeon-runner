using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Attach this to your Bomb prefab.
/// Player can ignite it by pressing E while standing nearby.
/// After a countdown, it explodes and destroys anything tagged "Explodable" within range.
/// </summary>
public class Bomb : MonoBehaviour
{
    [Tooltip("Key used to ignite the bomb.")]
    public KeyCode igniteKey = KeyCode.E;

    [Tooltip("How close the player must be to ignite the bomb.")]
    public float interactionRange = 2f;

    [Tooltip("Seconds between ignition and explosion.")]
    public float fuseTime = 5f;

    [Tooltip("How far the explosion reaches to destroy objects.")]
    public float explosionRadius = 3f;

    [Tooltip("Tag that marks objects that should be destroyed by the explosion (e.g. your sphere).")]
    public string explodableTag = "Explodable";

    [Tooltip("Optional: particle effect or object to spawn when the bomb explodes.")]
    public GameObject explosionEffectPrefab;

    // Other scripts (like BombPlacer) can subscribe to this to know when the bomb is gone
    public event Action OnBombDestroyed;

    private bool ignited = false;
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Bomb: No object tagged 'Player' found in the scene!");
        }
    }

    void Update()
    {
        if (ignited || player == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionRange && Input.GetKeyDown(igniteKey))
        {
            Ignite();
        }
    }

    void Ignite()
    {
        ignited = true;
        StartCoroutine(FuseCountdown());
    }

    IEnumerator FuseCountdown()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    void Explode()
    {
        // Optional visual effect
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Find everything within the explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag(explodableTag))
            {
                Destroy(hit.gameObject);
            }
        }

        // Let any listeners know the bomb is gone (e.g. BombPlacer)
        OnBombDestroyed?.Invoke();

        Destroy(gameObject);
    }

    // Draws the explosion radius as a wire sphere in the Scene view when this object is selected,
    // so you can visually see how far the explosion will reach while editing.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
