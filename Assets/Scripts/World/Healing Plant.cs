using UnityEngine;

/// <summary>
/// A plant the player can eat. Walk up, press E: the plant disappears and the player gets some hearts
/// back (2 by default), never going above Max Health.
///
/// Setup: put this on the plant with a BoxCollider ("Is Trigger" ticked, a bit bigger than the plant).
/// </summary>
public class HealingPlant : MonoBehaviour
{
    [SerializeField] private int heartsRestored = 2;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [Tooltip("Off = the player can eat it even at full health (it's wasted). On = it stays until they need it.")]
    [SerializeField] private bool onlyWhenHurt = true;

    private Health playerHealth;

    // NOTE: uses Unity's classic Input class. If Unity throws an error about it, set
    // Project Settings > Player > Active Input Handling to "Both".
    private void Update()
    {
        if (playerHealth != null && Input.GetKeyDown(interactKey))
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

        playerHealth.health = Mathf.Min(playerHealth.health + heartsRestored, playerHealth.maxHealth);
        Debug.Log("Ate a plant: health is now " + playerHealth.health);

        playerHealth = null; // stops a second press in the same frame
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
