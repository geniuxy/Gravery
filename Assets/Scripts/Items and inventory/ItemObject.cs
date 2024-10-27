using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private SpriteRenderer sr => GetComponent<SpriteRenderer>();

    private void OnValidate()
    {
        sr.sprite = itemData.itemIcon;
        gameObject.name = "Item - " + itemData.itemName;
    }

    private void SetupVisual()
    {
        if (itemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = itemData.itemIcon;
        gameObject.name = "Item - " + itemData.itemName;
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;
        SetupVisual();
    }

    public void PickUp()
    {
        // 有个弹起又落下的动作
        rb.velocity = new Vector2(0, 10);
        if (itemData.itemType == ItemType.Equipment &&
            !Inventory.instance.CanAddEquipment(itemData as ItemData_Equipment))
        {
            PlayerManager.instance.player.fx.CreatePopUpTextFX("inventory is full");
            return;
        }

        AudioManager.instance.PlaySFX(10);
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject, .2f);
    }
}