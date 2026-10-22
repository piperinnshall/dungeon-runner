using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to your Player object.
/// Holds a simple list of item names the player has collected.
/// Other scripts (like Treasure.cs) call AddItem() to add things to it.
/// </summary>
public class Inventory : MonoBehaviour
{
    [Tooltip("Items currently held by the player. You can see this fill up live in Play mode.")]
    public List<string> items = new List<string>();

    // Fired whenever an item is added or removed, so the HUD (e.g. KeyDisplay) can update.
    public event Action Changed;

    // Each level has its own player, so the items are kept here between scene transitions
    // (otherwise keys picked up in one level would vanish in the next). Reset every time Play is pressed.
    private static List<string> carriedItems = new List<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetCarriedItems() => carriedItems = new List<string>();

    void Awake()
    {
        // Pick up whatever the player was carrying in the previous level.
        items = new List<string>(carriedItems);
    }

    void SaveCarriedItems() => carriedItems = new List<string>(items);

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        SaveCarriedItems();
        Debug.Log("Picked up: " + itemName + " (Total items: " + items.Count + ")");
        Changed?.Invoke();
    }

    /// <summary>Removes ONE copy of the item. Returns false if the player didn't have it.</summary>
    public bool RemoveItem(string itemName)
    {
        bool removed = items.Remove(itemName);
        if (removed)
        {
            SaveCarriedItems();
            Debug.Log("Used: " + itemName + " (Total items: " + items.Count + ")");
            Changed?.Invoke();
        }
        return removed;
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public int CountOf(string itemName)
    {
        int count = 0;
        foreach (string item in items)
        {
            if (item == itemName)
                count++;
        }
        return count;
    }

    public void DisplayInventory()
    {
        Debug.Log("Inventory contains: " + string.Join(", ", items));
        // Insert UI here
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            DisplayInventory();
        }
    }
}
