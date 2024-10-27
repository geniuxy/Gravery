using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private int amountOfDrop;
    [SerializeField] private List<ItemData> possibleDropItems;
    private List<ItemData> dropItems = new List<ItemData>();

    public virtual void GenerateDrop()
    {
        while (dropItems.Count < amountOfDrop)
        {
            foreach (ItemData possibleDropItem in possibleDropItems)
            {
                if (Random.Range(0, 100) <= possibleDropItem.dropChance)
                    dropItems.Add(possibleDropItem);
            }
        }

        for (int i = 0; i < amountOfDrop; i++)
        {
            ItemData randomItem = dropItems[Random.Range(0, dropItems.Count - 1)];
            dropItems.Remove(randomItem);
            DropItem(randomItem);
        }
    }

    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);
        Vector2 randomVelocity = new Vector2(Random.Range(-5f, 5f), Random.Range(10f, 15f));
        newDrop.GetComponent<ItemObject>()?.SetupItem(_itemData, randomVelocity);
    }
}