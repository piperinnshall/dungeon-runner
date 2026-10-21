using UnityEngine;

public enum KeyType { Gold, Silver }

/// <summary>
/// Put this on a key object (with a BoxCollider that has "Is Trigger" ticked).
/// When the player walks into it (or presses the interact key, if Require Press is on), a gold or
/// silver key goes into their Inventory and the key object disappears.
/// Any gold key opens any gold lock, and any silver key opens any silver lock. Keys are used up.
/// </summary>
public class KeyPickup : MonoBehaviour
{
    [SerializeField] private KeyType keyType = KeyType.Gold;
    [Tooltip("Off = picked up as soon as the player touches it. On = player must also press the interact key.")]
    [SerializeField] private bool requirePress = false;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("Sound (optional)")]
    [SerializeField] private AudioClip pickupSound;
    [Range(0f, 1f)] [SerializeField] private float pickupVolume = 1f;

    /// <summary>The name a key of this type has in the Inventory ("GoldKey" or "SilverKey").</summary>
    public static string ItemName(KeyType type) => type == KeyType.Gold ? "GoldKey" : "SilverKey";

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

        inv.AddItem(ItemName(keyType));

        // PlayClipAtPoint keeps playing after the key object is destroyed.
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);

        Destroy(gameObject);
    }
}
