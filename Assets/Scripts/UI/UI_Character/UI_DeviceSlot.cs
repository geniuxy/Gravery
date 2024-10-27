using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_DeviceSlot : UI_ItemSlot
{
    public EquipmentType slotType;

    private void OnValidate() => gameObject.name = "Device - " + slotType;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.itemData == null)
            return;
        AudioManager.instance.PlaySFX(28, _allowSimultaneousSounds: true);
        Inventory.instance.UnequipItem(item.itemData as ItemData_Equipment);
        Inventory.instance.AddItem(item.itemData as ItemData_Equipment);
        ui_ItemTooltip.HideItemTooltip();
        CleanUpSlot();
    }
}