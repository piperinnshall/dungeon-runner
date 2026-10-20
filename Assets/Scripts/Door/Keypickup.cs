using UnityEngine;

public enum KeyType { Silver, Gold }

/// <summary>
/// Put this on the key object (with a BoxCollider that has "Is Trigger" ticked).
/// When the player walks into it (or presses the interact key, if requirePress is on),
/// the key is added to their Inventory and the key object disappears.
/// The keyId must match the "Required Key" on the Door.
/// </summary>
public class KeyPickup : MonoBehaviour
{
    [Tooltip("Must match the Door's 'Required Key' exactly (case-sensitive).")]
    [SerializeField] private string keyId = "RustyKey";
    [Tooltip("Which HUD counter this key shows up in.")]
    [SerializeField] private KeyType keyType = KeyType.Gold;
    [Tooltip("Off = picked up as soon as the player touches it. On = player must also press the interact key.")]
    [SerializeField] private bool requirePress = false;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    // Every key id that has been picked up and its type, so the HUD can tell which inventory items are
    // silver keys and which are gold keys.
    private static readonly System.Collections.Generic.Dictionary<string, KeyType> knownKeys =
        new System.Collections.Generic.Dictionary<string, KeyType>();

    /// <summary>True if this inventory item name came from a KeyPickup.</summary>
    public static bool IsKey(string itemName) => knownKeys.ContainsKey(itemName);

    /// <summary>True if this inventory item name is a key of the given type.</summary>
    public static bool IsKeyOfType(string itemName, KeyType type) =>
        knownKeys.TryGetValue(itemName, out KeyType t) && t == type;

    private Inventory playerInventory;
    private bool pickedUp;

    private void OnTriggerEnter(Collider other)
    {
        Inventory inv = other.GetComponentInParent<Inventory>();
        if (inv == null) return;

        if (requirePress) playerInventory = inv;
        else PickUp(inv);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Inventory>() == playerInventory) playerInventory = null;
    }

    private void Update()
    {
        if (requirePress && playerInventory != null && Input.GetKeyDown(interactKey))
            PickUp(playerInventory);
    }

    private void PickUp(Inventory inv)
    {
        if (pickedUp) return; // stops a double pickup if the player has more than one collider
        pickedUp = true;

        knownKeys[keyId] = keyType;
        inv.AddItem(keyId);
        Destroy(gameObject);
    }
}
