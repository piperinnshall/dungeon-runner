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
    [Tooltip("Seconds between ignition and explosion.")]
    public float fuseTime = 5f;

    [Tooltip("How far the explosion reaches to destroy objects.")]
    public float explosionRadius = 3f;

    [Tooltip("Tag that marks objects that should be destroyed by the explosion (e.g. your sphere).")]
    public string explodableTag = "Explodable";

    [Tooltip("Optional: particle effect or object to spawn when the bomb explodes.")]
    public GameObject explosionEffectPrefab;

    [Tooltip("Damage dealt to the player and to enemies caught in the explosion radius.")]
    public int explosionDamage = 2;

    [Tooltip("Optional: rubble effect spawned where each Explodable object breaks.")]
    public GameObject rubbleEffectPrefab;

    [Header("Fuse effect (optional)")]
    [Tooltip("A particle effect that is a CHILD of the bomb, placed on the fuse. Plays from ignition until the explosion.")]
    public ParticleSystem fuseEffect;

    [Header("Sounds (optional)")]
    [Tooltip("Loops while the fuse is burning.")]
    public AudioClip fuseSound;
    public AudioClip explosionSound;
    [Tooltip("Played where Explodable objects get destroyed.")]
    public AudioClip rubbleSound;
    [Range(0f, 1f)] public float fuseVolume = 0.6f;
    [Range(0f, 1f)] public float explosionVolume = 1f;
    [Range(0f, 1f)] public float rubbleVolume = 1f;

    // Other scripts (like BombPlacer) can subscribe to this to know when the bomb is gone
    public event Action OnBombDestroyed;

    private bool ignited = false;
    private Transform player;
    private AudioSource fuseSource;

    void Awake()
    {
        // The fuse effect shouldn't run until the bomb is lit.
        if (fuseEffect != null) fuseEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

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
    }

    public void Ignite()
    {
        if (ignited) return;
        ignited = true;

        // Fuse sparks + fuse sound until the explosion (both are destroyed with the bomb).
        if (fuseEffect != null) fuseEffect.Play(true);

        if (fuseSound != null)
        {
            fuseSource = gameObject.AddComponent<AudioSource>();
            fuseSource.clip = fuseSound;
            fuseSource.loop = true;
            fuseSource.volume = fuseVolume;
            fuseSource.spatialBlend = 1f; // 3D: comes from the bomb
            fuseSource.minDistance = 2f;
            fuseSource.maxDistance = 25f;
            fuseSource.Play();
        }

        StartCoroutine(FuseCountdown());
    }

    IEnumerator FuseCountdown()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    void Explode()
    {
        // Explosion sound. PlayClipAtPoint keeps playing after the bomb object is destroyed.
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);

        // Optional visual effect
        SpawnEffect(explosionEffectPrefab, transform.position);

        // Find everything within the explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        bool rubblePlayed = false;

        // Each character only takes damage once, even if several of its colliders are in range.
        System.Collections.Generic.HashSet<object> damaged = new System.Collections.Generic.HashSet<object>();

        foreach (Collider hit in hitColliders)
        {
            // Damage the player
            Health playerHealth = hit.GetComponentInParent<Health>();
            if (playerHealth != null && damaged.Add(playerHealth))
                playerHealth.TakeDamage(explosionDamage, transform.position);

            // Damage enemies (goblins, skeletons, anything with an EnemyHealth script)
            EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null && damaged.Add(enemyHealth))
                enemyHealth.TakeDamage(explosionDamage, transform.position);

            if (hit.CompareTag(explodableTag))
            {
                // One rubble sound per explosion, at the first thing destroyed (stops it getting
                // deafening when several rocks go at once).
                if (!rubblePlayed && rubbleSound != null)
                {
                    AudioSource.PlayClipAtPoint(rubbleSound, hit.transform.position, rubbleVolume);
                    rubblePlayed = true;
                }

                // Rubble burst from the middle of the object that broke.
                SpawnEffect(rubbleEffectPrefab, hit.bounds.center);

                // Remember it, so it stays destroyed when the player comes back to this level.
                WorldState.MarkDestroyed(hit.gameObject);
                Destroy(hit.gameObject);
            }
        }

        // Let any listeners know the bomb is gone
        OnBombDestroyed?.Invoke();

        Destroy(gameObject);
    }

    // Spawns a particle effect prefab, plays EVERY particle system in it (effects are often several
    // systems as children) and removes it once the longest one has finished.
    // Uses the prefab's own rotation: many effects are rotated -90 on X to point upwards, and
    // spawning with Quaternion.identity threw that away and laid the effect on its side.
    void SpawnEffect(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;

        GameObject effect = Instantiate(prefab, position, prefab.transform.rotation);
        ParticleSystem[] systems = effect.GetComponentsInChildren<ParticleSystem>();

        if (systems.Length == 0)
        {
            Destroy(effect, 5f);
            return;
        }

        float duration = 0f;
        foreach (ParticleSystem ps in systems)
        {
            ps.Play(false);
            duration = Mathf.Max(duration, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        Destroy(effect, duration);
    }

    // Draws the explosion radius as a wire sphere in the Scene view when this object is selected,
    // so you can visually see how far the explosion will reach while editing.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
