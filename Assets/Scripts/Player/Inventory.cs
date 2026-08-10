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

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log("Picked up: " + itemName + " (Total items: " + items.Count + ")");
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
}
