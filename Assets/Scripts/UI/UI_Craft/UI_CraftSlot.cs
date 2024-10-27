using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{
    protected override void Start()
    {
        base.Start();
    }

    public void SetupCraftSlot(ItemData _data)
    {
        if (_data == null)
            return;

        item.itemData = _data;

        itemImage.sprite = _data.itemIcon;

        itemText.text = _data.itemName;
        itemText.fontSize = itemText.text.Length > 10 ? 18 : 24;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        UI_CraftWindow uiCraftWindow = UI.instance.ui_CraftWindow;
        if (!uiCraftWindow.gameObject.activeSelf)
            uiCraftWindow.gameObject.SetActive(true);
        uiCraftWindow.SetupCraftWindow(item.itemData as ItemData_Equipment);
    }
}