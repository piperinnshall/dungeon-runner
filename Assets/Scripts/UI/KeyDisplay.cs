using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD: shows one icon for every key of ONE type (gold or silver) the player carries, side by side
/// like the hearts. Used keys disappear because the Door removes them from the Inventory.
///
/// Setup: put this on an empty UI object with a Horizontal Layout Group (e.g. "GoldKeys").
/// Give it ONE child Image with that key's sprite and drag the child into "Key Icon Template".
/// Make one of these for gold keys and another for silver keys.
/// </summary>
public class KeyDisplay : MonoBehaviour
{
    [SerializeField] private KeyType keyType = KeyType.Gold;
    [Tooltip("A child Image with the key sprite. It is used as the template and hidden itself.")]
    [SerializeField] private Image keyIconTemplate;

    private readonly List<GameObject> icons = new List<GameObject>();
    private Inventory inventory;

    private void Start()
    {
        if (keyIconTemplate != null) keyIconTemplate.gameObject.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) inventory = player.GetComponentInChildren<Inventory>();

        if (inventory == null)
        {
            Debug.LogWarning("KeyDisplay: couldn't find an Inventory on the object tagged 'Player'.", this);
            return;
        }

        inventory.Changed += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (inventory != null) inventory.Changed -= Refresh;
    }

    private void Refresh()
    {
        if (keyIconTemplate == null) return;

        int count = 0;
        foreach (string item in inventory.items)
            if (KeyPickup.IsKeyOfType(item, keyType)) count++;

        // Make sure there are enough icons, then show exactly 'count' of them.
        while (icons.Count < count)
            icons.Add(Instantiate(keyIconTemplate.gameObject, keyIconTemplate.transform.parent));

        for (int i = 0; i < icons.Count; i++)
            icons[i].SetActive(i < count);
    }
}
