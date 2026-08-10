using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //public List<Item> items = new List<Item>();
    public int totalTreasureValue = 0;
    public int money = 0;
    public int itemWeight = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //   public bool AddItem(Item newItem)
    //   {
    //       if (newItem.type() == treasure)
    //       {
    //           items.add(newItem);
    //           totalTreasureValue += newItem.getValue();
    //       } else
    //       {
    //           items.add(newItem);
    //       }
    //       itemWeight += newItem.weight();
    //   }
}
