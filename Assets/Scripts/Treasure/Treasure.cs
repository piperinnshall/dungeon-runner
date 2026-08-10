using UnityEngine;

/// <summary>
/// Attach this to any treasure/item object in your level.
/// When the player is close and presses E, it's added to their Inventory and removed from the scene.
/// </summary>
public class Treasure : MonoBehaviour
{
    [Tooltip("Name of this item, used when adding it to the player's inventory.")]
    public string itemName = "Treasure";

    [Tooltip("Key used to pick up this item.")]
    public KeyCode pickupKey = KeyCode.E;

    [Tooltip("How close the player must be to pick this up.")]
    public float interactionRange = 2f;

    [Tooltip("Optional: effect or sound object to spawn when picked up.")]
    public GameObject pickupEffectPrefab;

    private Transform player;
    private Inventory playerInventory;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerInventory = playerObj.GetComponent<Inventory>();

            if (playerInventory == null)
            {
                Debug.LogWarning("Treasure: Player object has no Inventory component attached!");
            }
        }
        else
        {
            Debug.LogWarning("Treasure: No object tagged 'Player' found in the scene!");
        }
    }

    void Update()
    {
        if (player == null || playerInventory == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionRange && Input.GetKeyDown(pickupKey))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        playerInventory.AddItem(itemName);

        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // Draws the interaction range as a yellow wire sphere in the Scene view when selected
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
