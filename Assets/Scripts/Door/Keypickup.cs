using UnityEngine;

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
    [Tooltip("Off = picked up as soon as the player touches it. On = player must also press the interact key.")]
    [SerializeField] private bool requirePress = false;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    private Inventory playerInventory;

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
        inv.AddItem(keyId);
        Destroy(gameObject);
    }
}