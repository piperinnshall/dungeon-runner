using System.Collections;
using UnityEngine;

/// <summary>
/// A plant the player can eat. Walk up, press E: the plant disappears with a pick-up sound, then the
/// player gets some hearts back (2 by default, never above Max Health) with a heal sound.
///
/// Setup: put this on the plant with a BoxCollider ("Is Trigger" ticked, a bit bigger than the plant).
/// </summary>
public class HealingPlant : MonoBehaviour
{
    [SerializeField] private int heartsRestored = 2;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [Tooltip("Off = the player can eat it even at full health (it's wasted). On = it stays until they need it.")]
    [SerializeField] private bool onlyWhenHurt = true;

    [Header("Sounds (optional)")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip healSound;
    [Tooltip("Seconds between the pick-up sound and the hearts coming back (with the heal sound). Negative = wait for the pick-up sound to finish.")]
    [SerializeField] private float healDelay = -1f;
    [Range(0f, 1f)] [SerializeField] private float volume = 1f;

    private Health playerHealth;
    private bool eaten;

    // NOTE: uses Unity's classic Input class. If Unity throws an error about it, set
    // Project Settings > Player > Active Input Handling to "Both".
    private void Update()
    {
        if (!eaten && playerHealth != null && Input.GetKeyDown(interactKey))
            Eat();
    }

    private void Eat()
    {
        if (playerHealth.isDead) return;

        if (onlyWhenHurt && playerHealth.health >= playerHealth.maxHealth)
        {
            Debug.Log("Already at full health.");
            return;
        }

        eaten = true;
        StartCoroutine(EatRoutine(playerHealth));
    }

    private IEnumerator EatRoutine(Health health)
    {
        // Make the plant vanish straight away, but keep this object alive long enough to finish the sounds.
        foreach (Renderer r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // the player is standing on it, so 2D

        if (pickupSound != null) source.PlayOneShot(pickupSound, volume);

        float wait = healDelay >= 0f ? healDelay : (pickupSound != null ? pickupSound.length : 0f);
        if (wait > 0f) yield return new WaitForSeconds(wait);

        if (health != null && !health.isDead)
        {
            health.health = Mathf.Min(health.health + heartsRestored, health.maxHealth);
            Debug.Log("Ate a plant: health is now " + health.health);
        }

        if (healSound != null)
        {
            source.PlayOneShot(healSound, volume);
            yield return new WaitForSeconds(healSound.length);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerHealth = other.GetComponentInParent<Health>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerHealth = null;
    }
}
