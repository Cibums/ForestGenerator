using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public int id;
    public InventoryItemType type;
}

public enum InventoryItemType
{
    Item,
    Building,
}
