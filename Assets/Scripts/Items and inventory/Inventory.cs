using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> deviceItems;
    public Dictionary<ItemData_Equipment, InventoryItem> deviceDictionary;

    public List<InventoryItem> equipmentItems;
    public Dictionary<ItemData, InventoryItem> equipmentDictionary;

    public List<InventoryItem> stashItems;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform equipmentSlotParent; //指几个物品槽的父节点
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform deviceSlotParent;
    [SerializeField] private Transform statSlotParent;
    private UI_ItemSlot[] equipmentItemSlots; // 装备物品槽
    private UI_ItemSlot[] stashItemSlots; // 材料物品槽
    private UI_DeviceSlot[] deviceItemSlots;
    private UI_StatSlot[] statSlots;

    [Header("Data base")]
    public List<ItemData> itemDataBases;
    public List<InventoryItem> loadedItems; // 包括stash和equipment
    public List<ItemData_Equipment> loadedDevices;

    public float flaskCoolDown { get; private set; }
    private float lastTimeUseFlask;
    private float armorCoolDown;
    private float lastTimeUseArmor;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;
    }

    private void Start()
    {
        deviceItems = new List<InventoryItem>();
        deviceDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();
        equipmentItems = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData, InventoryItem>();
        stashItems = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipmentItemSlots = equipmentSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlots = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        deviceItemSlots = deviceSlotParent.GetComponentsInChildren<UI_DeviceSlot>();
        statSlots = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        Invoke(nameof(ApplyStartingItem), .05f);
    }

    private void ApplyStartingItem()
    {
        foreach (var loadedDevice in loadedDevices)
            DeployDevice(loadedDevice);
        // 装备之后调整血量
        PlayerManager.instance.player.stats.currentHp =
            PlayerManager.instance.player.stats.GetMaxHpValue();

        if (loadedItems.Count > 0)
        {
            LoadEquipmentData();
            return;
        }

        foreach (ItemData item in startingItems)
            if (item != null)
                AddItem(item);
    }

    private void LoadEquipmentData()
    {
        foreach (var loadedEquipment in loadedItems)
        {
            for (int i = 0; i < loadedEquipment.stackSize; i++)
            {
                AddItem(loadedEquipment.itemData);
            }
        }
    }

    public void UpdateSlotUI()
    {
        // 更新装备槽
        foreach (var deviceItemSlot in deviceItemSlots)
            deviceItemSlot.CleanUpSlot();
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> deivce in deviceDictionary)
        {
            foreach (var deviceItemSlot in deviceItemSlots)
            {
                if (deivce.Key.equipmentType == deviceItemSlot.slotType)
                    deviceItemSlot.UpdateSlot(deivce.Value);
            }
        }

        // 清理物品槽
        foreach (var equipmentItemSlot in equipmentItemSlots)
            equipmentItemSlot.CleanUpSlot();
        foreach (var stashItemSlot in stashItemSlots)
            stashItemSlot.CleanUpSlot();
        // 更新物品槽
        for (int i = 0; i < equipmentItems.Count; i++)
            equipmentItemSlots[i].UpdateSlot(equipmentItems[i]);
        for (int i = 0; i < stashItems.Count; i++)
            stashItemSlots[i].UpdateSlot(stashItems[i]);

        UpdataStatsUI();
    }

    public void UpdataStatsUI()
    {
        // 更新状态槽
        foreach (var statSlot in statSlots)
            statSlot.UpdateStatSlotUI();
    }

    public void DeployDevice(ItemData _itemData)
    {
        ItemData_Equipment newDevice = _itemData as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newDevice);
        ItemData_Equipment oldEquipment = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> deivce in deviceDictionary)
        {
            if (newDevice != null && deivce.Key.equipmentType == newDevice.equipmentType)
                oldEquipment = deivce.Key;
        }

        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);
        }

        deviceItems.Add(newItem);
        deviceDictionary.Add(newDevice, newItem);
        newDevice.AddModifiers();
        RemoveItem(_itemData);
    }

    public void UnequipItem(ItemData_Equipment ItemToUnequip)
    {
        if (deviceDictionary.TryGetValue(ItemToUnequip, out InventoryItem value))
        {
            deviceItems.Remove(value);
            deviceDictionary.Remove(ItemToUnequip);
            ItemToUnequip.RemoveModifiers();
        }
    }

    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddEquipment(_item as ItemData_Equipment))
            AddToEquipment(_item);
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);

        UpdateSlotUI();
    }

    private void AddToEquipment(ItemData _item)
    {
        if (equipmentDictionary.TryGetValue(_item, out InventoryItem value))
            value.AddStack();
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            equipmentItems.Add(newItem);
            equipmentDictionary.Add(_item, newItem);
        }
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
            value.AddStack();
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stashItems.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment)
            RemoveFromEquipment(_item);
        else if (_item.itemType == ItemType.Material)
            RemoveFromStash(_item);

        UpdateSlotUI();
    }

    private void RemoveFromEquipment(ItemData _item)
    {
        if (equipmentDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                equipmentItems.Remove(value);
                equipmentDictionary.Remove(_item);
            }
            else
                value.RemoveStack();
        }
    }

    private void RemoveFromStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                stashItems.Remove(value);
                stashDictionary.Remove(_item);
            }
            else
                value.RemoveStack();
        }
    }

    public bool CanAddEquipment(ItemData_Equipment equipmentData)
    {
        if (!IsEquipExisted(equipmentData) && equipmentItems.Count >= equipmentItemSlots.Length)
        {
            Debug.Log("No more space");
            return false;
        }

        return true;
    }

    private bool IsEquipExisted(ItemData_Equipment equipmentData)
    {
        foreach (var equipmentItem in equipmentItems)
        {
            if (equipmentData == equipmentItem.itemData)
                return true;
        }

        return false;
    }

    public bool CanCraftEquipment(ItemData_Equipment _itemToCraft)
    {
        List<InventoryItem> craftMaterials = _itemToCraft.craftMaterials;
        if (!IsMaterialsEnough(craftMaterials)) return false;

        UpdateInventoryWhenCraft(_itemToCraft);
        return true;
    }

    private void UpdateInventoryWhenCraft(ItemData_Equipment _itemToCraft)
    {
        foreach (InventoryItem craftMaterial in _itemToCraft.craftMaterials)
        {
            for (int i = 0; i < craftMaterial.stackSize; i++)
                RemoveFromStash(craftMaterial.itemData);
        }

        AddItem(_itemToCraft);
    }

    private bool IsMaterialsEnough(List<InventoryItem> craftMaterials)
    {
        foreach (InventoryItem craftMaterial in craftMaterials)
        {
            if (stashDictionary.TryGetValue(craftMaterial.itemData, out InventoryItem stashValue))
            {
                if (stashValue.stackSize < craftMaterial.stackSize)
                {
                    Debug.Log("Not enough materials: " + craftMaterial.itemData.itemName);
                    return false;
                }
            }
            else
            {
                Debug.Log("Material not found: " + craftMaterial.itemData.itemName);
                return false;
            }
        }

        return true;
    }

    public List<InventoryItem> GetDeviceList() => deviceItems;

    public List<InventoryItem> GetEquipmentList() => equipmentItems;

    public List<InventoryItem> GetMaterialList() => stashItems;

    public ItemData_Equipment GetDevice(EquipmentType _equipmentType)
    {
        ItemData_Equipment equipment = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> deivce in deviceDictionary)
        {
            if (deivce.Key.equipmentType == _equipmentType)
                equipment = deivce.Key;
        }

        return equipment;
    }

    public void UseFlask()
    {
        ItemData_Equipment currentFlask = GetDevice(EquipmentType.Flask);

        if (currentFlask == null)
            return;

        bool canUseFlask = Time.time > lastTimeUseFlask + flaskCoolDown;

        if (canUseFlask)
        {
            flaskCoolDown = currentFlask.coolDown;
            currentFlask.ExecuteEffect(null);
            lastTimeUseFlask = Time.time;

            AudioManager.instance.PlaySFX(23);
        }
        else
            PlayerManager.instance.player.fx.CreatePopUpTextFX("cooling down");
    }

    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetDevice(EquipmentType.Armor);
        if (currentArmor == null)
            return false;

        if (Time.time > lastTimeUseArmor + armorCoolDown)
        {
            armorCoolDown = currentArmor.coolDown;
            lastTimeUseArmor = Time.time;
            return true;
        }

        Debug.Log("Armor Effect can't use now!");
        return false;
    }

    public void LoadData(GameData _data)
    {
        foreach (var pair in _data.inventory)
        {
            foreach (var itemData in itemDataBases)
            {
                if (itemData == null) // 如果遇到菜单的相关数据(此时data为null)，就跳过
                    continue;
                if (itemData.itemID == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(itemData);
                    itemToLoad.stackSize = pair.Value;
                    loadedItems.Add(itemToLoad);
                }
            }
        }

        foreach (var deviceId in _data.deviceIds)
        {
            foreach (var itemData in itemDataBases)
            {
                if (itemData == null) // 如果遇到菜单的相关数据(此时data为null)，就跳过
                    continue;
                if (itemData.itemID == deviceId)
                {
                    ItemData_Equipment deviceToLoad = itemData as ItemData_Equipment;
                    loadedDevices.Add(deviceToLoad);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.deviceIds.Clear();

        foreach (var pair in equipmentDictionary)
        {
            _data.inventory.Add(pair.Key.itemID, pair.Value.stackSize);
        }

        foreach (var pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemID, pair.Value.stackSize);
        }

        foreach (var pair in deviceDictionary)
        {
            _data.deviceIds.Add(pair.Key.itemID);
        }
    }

# if UNITY_EDITOR
    [ContextMenu("Fill up item data base when beta")]
    private void FillUpItemDataBases() => itemDataBases = new List<ItemData>(GetItemDataBase());

    // 获取Assets/Data/Items目录下的所有Item
    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (var assetName in assetNames)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetName);
            ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
#endif
}