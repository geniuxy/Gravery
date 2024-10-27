using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    // 物品
    public ItemData itemData;
    // 物品数量
    public int stackSize;

    public InventoryItem(ItemData _itemData)
    {
        itemData = _itemData;
        AddStack();
    }

    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;
}
