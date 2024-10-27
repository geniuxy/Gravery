using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    public InventoryItem item;

    protected UI ui;
    protected UI_ItemTooltip ui_ItemTooltip;

    protected virtual void Start()
    {
        ui = UI.instance;
        ui_ItemTooltip = ui.ui_ItemTooltip;
    }

    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;

        if (item != null)
        {
            itemImage.sprite = item.itemData.itemIcon;
            itemImage.color = Color.white;
            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
                itemText.text = "";
        }
    }

    public void CleanUpSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.itemData == null)
            return;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.itemData);
            return;
        }

        if (item.itemData.itemType == ItemType.Equipment)
        {
            Inventory.instance.DeployDevice(item.itemData);
            AudioManager.instance.PlaySFX(27, _allowSimultaneousSounds: true);
        }

        ui_ItemTooltip.HideItemTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null || item.itemData == null)
            return;
        if (item.itemData.itemType == ItemType.Material)
            return;
        Vector2 mousePos = Input.mousePosition;
        float xOffset = mousePos.x > Screen.width * .5 ? -50 : 50;
        float yOffset = mousePos.y > Screen.height * .5 ? -50 : 50;
        ui_ItemTooltip.transform.position = new Vector2(mousePos.x + xOffset, mousePos.y + yOffset);
        ui_ItemTooltip.ShowItemTooltip(item.itemData as ItemData_Equipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null || item.itemData == null)
            return;
        ui_ItemTooltip.HideItemTooltip();
    }
}