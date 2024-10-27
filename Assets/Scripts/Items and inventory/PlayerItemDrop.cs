using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerItemDrop : ItemDrop
{
    private Inventory inventory;

    [Header("Player drop info")]
    [SerializeField] private float chanceToLooseDevice;
    [SerializeField] private float chanceToLooseEquipment;
    [SerializeField] private float chanceToLooseMaterial;

    [Header("Devices UI")]
    [SerializeField] private Transform deviceSlotParent;
    private UI_DeviceSlot[] deviceItemSlots;

    private void Start()
    {
        inventory = Inventory.instance;
        deviceItemSlots = deviceSlotParent.GetComponentsInChildren<UI_DeviceSlot>();
    }

    public override void GenerateDrop()
    {
        DropDevice();
        DropEquipment();
        DropMaterial();
    }

    private void DropDevice()
    {
        List<ItemData_Equipment> deviceToUnequip = new List<ItemData_Equipment>();

        foreach (InventoryItem device in inventory.GetDeviceList())
        {
            if (Random.Range(0, 100) < chanceToLooseDevice)
            {
                ItemData_Equipment deviceItemData = device.itemData as ItemData_Equipment;
                DropItem(deviceItemData);
                deviceToUnequip.Add(deviceItemData);
            }
        }

        foreach (ItemData_Equipment device in deviceToUnequip)
        {
            inventory.UnequipItem(device);
            // 死亡掉落装备更新图标
            CleanUpDeviceUI(device);
        }
    }

    private void CleanUpDeviceUI(ItemData_Equipment device)
    {
        foreach (var deviceItemSlot in deviceItemSlots)
        {
            if (device != null && deviceItemSlot.slotType == device.equipmentType)
                deviceItemSlot.CleanUpSlot();
        }
    }

    private void DropEquipment()
    {
        List<ItemData> equipmentToRemove = new List<ItemData>();

        foreach (InventoryItem equipment in inventory.GetEquipmentList())
        {
            if (Random.Range(0, 100) < chanceToLooseEquipment)
            {
                ItemData equipmentItemData = equipment.itemData;
                DropItem(equipmentItemData);
                equipmentToRemove.Add(equipmentItemData);
            }
        }

        foreach (ItemData equip in equipmentToRemove)
            inventory.RemoveItem(equip);
    }

    private void DropMaterial()
    {
        List<ItemData> materialToRemove = new List<ItemData>();

        foreach (InventoryItem material in inventory.GetMaterialList())
        {
            if (Random.Range(0, 100) < chanceToLooseMaterial)
            {
                ItemData materialItemData = material.itemData;
                DropItem(materialItemData);
                materialToRemove.Add(materialItemData);
            }
        }

        foreach (ItemData material in materialToRemove)
            inventory.RemoveItem(material);
    }
}