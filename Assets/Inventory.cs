using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>();
    public static Inventory ins;

    private void Awake()
    {
        if (ins == null)
        {
            ins = this;
        }    
    }

    public static void Add(Item item)
    {
        InventoryItem ii = new InventoryItem();
        ii.id = GameController.ins.GetItemID(item);
        ii.type = InventoryItemType.Item;

        ins.items.Add(ii);
    }

    public static void Add(int itemID)
    {
        InventoryItem ii = new InventoryItem();
        ii.id = itemID;
        ii.type = InventoryItemType.Item;

        ins.items.Add(ii);
    }
}
